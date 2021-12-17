using System.Collections.Concurrent;
using System.Threading.Channels;

namespace SlidingWindowCounters;

internal record RequestCounter
{
	public long TimeStamp { get; init; }
	public int Count { get; set; }
}

internal class UserRequests
{
	private readonly TimeSpan _window;
	private readonly int _maxRequest;
	private RequestCounter? _first = null;
	private RequestCounter? _second = null;

	private Channel<(TaskCompletionSource<bool> promise, DateTime timeStamp)> _channel = Channel.CreateUnbounded<(TaskCompletionSource<bool> promise, DateTime timeStamp)>();
	public UserRequests(TimeSpan window, int maxRequest)
	{
		_window = window;
		_maxRequest = maxRequest;

		Task.Run(async () =>
		{
			await foreach (var (promise, timeStamp) in _channel.Reader.ReadAllAsync())
			{
				VerifyAndAddRequest(promise, timeStamp);
			}
		});
	}

	private void VerifyAndAddRequest(TaskCompletionSource<bool> promise, DateTime timeStamp)
	{

		var current = new DateTime(
			timeStamp.Ticks - timeStamp.Ticks % _window.Ticks,
			timeStamp.Kind
		).Ticks;

		int GetCount()
		{
			var prevWeight = 1 - (timeStamp.Ticks - current) / (double)_window.Ticks;
			var i = (int)(_first.Count * prevWeight + _second.Count);
			return i;
		}

		if (_first == null)
		{
			_first = new RequestCounter { TimeStamp = current, Count = 1 };
			promise.SetResult(true);
		}
		else if (_first.TimeStamp == current)
		{
			_first.Count++;
			promise.SetResult(_first.Count <= _maxRequest);
		}
		else if (current - _first.TimeStamp > _window.Ticks)
		{
			if (_second != null && current - _second.TimeStamp == _window.Ticks)
			{
				_first = _second;
				_second = new RequestCounter { TimeStamp = current, Count = 1 };
				var count = GetCount();
				promise.SetResult(count <= _maxRequest);
			}
			else
			{
				_first = new RequestCounter { TimeStamp = current, Count = 1 };
				_second = null;
				promise.SetResult(true);
			}
		}
		else if (current - _first.TimeStamp == _window.Ticks)
		{
			if (_second == null)
				_second = new RequestCounter { TimeStamp = current, Count = 1 };
			else
				_second.Count++;
			
			var count = GetCount();
			promise.SetResult(count <= _maxRequest);
		}
	}

	public async Task<bool> TryAdd(DateTime timeStamp)
	{
			var promise = new TaskCompletionSource<bool>();
			await _channel.Writer.WriteAsync((promise, timeStamp));
			return await promise.Task;
	}

}


public class RateLimiter
{
	private readonly TimeSpan _window;
	private readonly int _maxRequest;
	private readonly ConcurrentDictionary<string, UserRequests> _userRequestsDict = new();

	public RateLimiter(TimeSpan window, int maxRequest)
	{
		_window = window;
		_maxRequest = maxRequest;
	}
	
	public async Task<bool> VerifyAndAddRequest(string userId, DateTime timeStamp)
	{
		var requests = _userRequestsDict.GetOrAdd(userId, _ => new UserRequests(_window, _maxRequest));
		return await requests.TryAdd(timeStamp);
	}
}