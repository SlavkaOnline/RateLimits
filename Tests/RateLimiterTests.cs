using System;
using System.Linq;
using System.Threading.Tasks;
using SlidingWindowCounters;
using Xunit;

namespace Tests;

public class RateLimiterTests
{
	[Fact]
	public async Task SimpleTest()
	{
		const int maxRequests = 10;
		var rateLimiter = new RateLimiter(TimeSpan.FromSeconds(1), maxRequests);
		var userId = Guid.NewGuid().ToString("N");
		var timeStamp = DateTime.UtcNow;
		var tasksResult = await Task.WhenAll(Enumerable.Range(0, 15)
			.Select(_ => rateLimiter.VerifyAndAddRequest(userId, timeStamp)));
			
		var result = tasksResult.Aggregate(0, (count, r) => count + (r ? 1 : 0));
		
		Assert.Equal(maxRequests, result);
	}
	
	[Fact]
	public async Task SimpleTest_notReject()
	{
		const int maxRequests = 10;
		var rateLimiter = new RateLimiter(TimeSpan.FromSeconds(1), maxRequests);
		var userId = Guid.NewGuid().ToString("N");
		var timeStamp = DateTime.UtcNow;
		var tasksResult = await Task.WhenAll(Enumerable.Range(0, 5)
			.Select(_ => rateLimiter.VerifyAndAddRequest(userId, timeStamp)));
			
		var result = tasksResult.Aggregate(0, (count, r) => count + (r ? 1 : 0));
		
		Assert.Equal(5, result);
	}
	
	[Fact]
	public async Task SimpleNotConcurrencyTestWithLargePause()
	{
		const int maxRequests = 10;
		var rateLimiter = new RateLimiter(TimeSpan.FromSeconds(1), maxRequests);
		var userId = Guid.NewGuid().ToString("N");
		var timeStamp = DateTime.UtcNow;
		await Task.WhenAll(Enumerable.Range(0, 10)
			.Select(_ => rateLimiter.VerifyAndAddRequest(userId, timeStamp)));
			
		var result = await rateLimiter.VerifyAndAddRequest(userId, timeStamp.AddSeconds(2));
		
		Assert.True(result);
	}
	
	[Fact]
	public async Task MultipleSecondsTest()
	{
		const int maxRequests = 10;
		var rateLimiter = new RateLimiter(TimeSpan.FromMinutes(1), maxRequests);
		var userId = Guid.NewGuid().ToString("N");
		var timeStamp = new DateTime(2021, 1, 1, 0, 0, 0);
		await Task.WhenAll(Enumerable.Range(0, 7)
			.Select(_ => rateLimiter.VerifyAndAddRequest(userId, timeStamp)));

		timeStamp = timeStamp.AddSeconds(15).AddMinutes(1);
		
		await Task.WhenAll(Enumerable.Range(0, 6)
			.Select(_ => rateLimiter.VerifyAndAddRequest(userId, timeStamp)));

		var result = await rateLimiter.VerifyAndAddRequest(userId, timeStamp);
		//7 *.75 + 7 = 12.25 > 10
		Assert.False(result);
	}
	
	[Fact]
	public async Task MultipleTest_notReject()
	{
		const int maxRequests = 13;
		var rateLimiter = new RateLimiter(TimeSpan.FromMinutes(1), maxRequests);
		var userId = Guid.NewGuid().ToString("N");
		var timeStamp = new DateTime(2021, 1, 1, 0, 0, 0);
		foreach (var second in Enumerable.Range(0, 7))
		{
			await rateLimiter.VerifyAndAddRequest(userId, timeStamp.AddSeconds(second));
		}
	
		timeStamp = timeStamp.AddSeconds(15).AddMinutes(1);
		
		foreach (var _ in Enumerable.Range(0, 6))
		{
		 	await rateLimiter.VerifyAndAddRequest(userId, timeStamp);
		}
		
		var result = await rateLimiter.VerifyAndAddRequest(userId, timeStamp);
		//7 *.75 + 7 = 12.25 < 13
		Assert.True(result);
	}
}