# OpenTracing / Jaeger Demo

Commands to setup the application:
```
dotnet new webapi -n opentracing-demo
cd opentracing-demo
dotnet add package Jaeger
dotnet add package OpenTracing.Contrib.NetCore
```


## With dotnet installed locally:
```
docker-compose up jaeger
env ADD_OPENTRACING=True JAEGER_AGENT_HOST=localhost dotnet run
for x in `seq 1 5`; do curl -s http://localhost:5000/api/values | grep --color -E 'value'; sleep .4; done;

env ADD_OPENTRACING=False dotnet run
for x in `seq 1 5`; do curl -s http://localhost:5000/api/values | grep --color -E 'value'; sleep .4; done;
```

## With Docker (no local dotnet required)
but then I ran this in docker and it seems to be working, so I wonder if it could be an issue with the Mac OSX environment?

```
# Start 2 versions of the service on ports 5002 (OpenTracing enabled) & 5003 (OpenTracing disabled)
docker-compose up --build

# Start a version on the local machine (in my case Mac OS)
env ADD_OPENTRACING=True JAEGER_AGENT_HOST=localhost dotnet run
```

Then I ran the following:
```
# Running against local instance with OpenTracing enabled, only resulted in 1 trace (with 4 spans)
for x in `seq 1 5`; do curl -s http://localhost:5000/api/values | grep --color -E 'value'; sleep .4; done;

# Running against docker instance with OpenTracing enabled, resulted in 5 traces (each with 4 spans)
for x in `seq 1 5`; do curl -s http://localhost:5002/api/values | grep --color -E 'value'; sleep .4; done;

# Running against docker instance with OpenTracing disabled, resulted in 5 traces (each with 1 spans)
for x in `seq 1 5`; do curl -s http://localhost:5003/api/values | grep --color -E 'value'; sleep .4; done;
```