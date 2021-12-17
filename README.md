> test suite: `nbomber_default_test_suite_name`

> test name: `nbomber_default_test_name`

> scenario: `InjectPerSec`, duration: `00:01:00`, ok count: `251`, fail count: `356`, all data: `0` MB MB

load simulation: `inject_per_sec`, rate: `10`, during: `00:01:00`
|step|ok stats|
|---|---|
|name|`Get`|
|request count|all = `607`, ok = `251`, RPS = `4.2`|
|latency|min = `0.43`, mean = `6.45`, max = `151.91`, StdDev = `25.07`|
|latency percentile|50% = `0.8`, 75% = `1.06`, 95% = `21.38`, 99% = `149.5`|

|step|fail stats|
|---|---|
|name|`Get`|
|request count|all = `607`, fail = `356`, RPS = `5.9`|
|latency|min = `3.18`, mean = `37.23`, max = `153.86`, StdDev = `26.17`|
|latency percentile|50% = `32.49`, 75% = `45.15`, 95% = `75.78`, 99% = `147.2`|
> status codes for scenario: `InjectPerSec`

|status code|count|message|
|---|---|---|
|ok (no status)|251||
|-101|356|step unhandled exception: One or more errors occurred. (Error while copying content to a stream.)|

> scenario: `InjectPerSecRandom`, duration: `00:01:00`, ok count: `246`, fail count: `357`, all data: `0` MB MB

load simulation: `inject_per_sec_random`, min rate: `1`, max rate: `20`, during: `00:01:00`
|step|ok stats|
|---|---|
|name|`Get`|
|request count|all = `603`, ok = `246`, RPS = `4.1`|
|latency|min = `0.42`, mean = `6.59`, max = `151.94`, StdDev = `26.52`|
|latency percentile|50% = `0.92`, 75% = `1.89`, 95% = `21.26`, 99% = `151.68`|

|step|fail stats|
|---|---|
|name|`Get`|
|request count|all = `603`, fail = `357`, RPS = `6`|
|latency|min = `4.59`, mean = `55.51`, max = `147.07`, StdDev = `32.54`|
|latency percentile|50% = `48.83`, 75% = `79.87`, 95% = `111.55`, 99% = `137.6`|
> status codes for scenario: `InjectPerSecRandom`

|status code|count|message|
|---|---|---|
|ok (no status)|246||
|-101|357|step unhandled exception: One or more errors occurred. (Error while copying content to a stream.)|

> scenario: `KeepConstant`, duration: `00:01:00`, ok count: `10`, fail count: `17771`, all data: `0` MB MB

load simulation: `keep_constant`, copies: `5`, during: `00:01:00`
|step|ok stats|
|---|---|
|name|`Get`|
|request count|all = `17781`, ok = `10`, RPS = `0.2`|
|latency|min = `1.54`, mean = `65.96`, max = `149.46`, StdDev = `66.37`|
|latency percentile|50% = `1.92`, 75% = `149.5`, 95% = `149.5`, 99% = `149.5`|

|step|fail stats|
|---|---|
|name|`Get`|
|request count|all = `17781`, fail = `17771`, RPS = `296.2`|
|latency|min = `5.64`, mean = `16.77`, max = `213.43`, StdDev = `13.43`|
|latency percentile|50% = `11.08`, 75% = `25.6`, 95% = `32.77`, 99% = `47.36`|
> status codes for scenario: `KeepConstant`

|status code|count|message|
|---|---|---|
|ok (no status)|10||
|-101|17771|step unhandled exception: One or more errors occurred. (Error while copying content to a stream.)|