
HBaseStargate.net is .net v3.5 library for Stargate(a RESTful Web service front end for HBase.) It's compatible with Stargate API in Hbase 0.20.2

It contains two projects first - HBaseStargate, main library, and second - HBaseStargateUsage, a few samples showing how to use HBaseStargate API

If you want run this you have to:
1. Start stargate on yours hbase cluster (http://wiki.apache.org/hadoop/Hbase/Stargate#A2)
2. run hbase and change host and port variables
3. create 'users' table in hbase, for this task you can use *insertScritp.txt* in main folder

TODO:
- add scanners
- add table operation (create,drop itp)