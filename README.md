# Rest api test app

[![Build status](https://ci.appveyor.com/api/projects/status/wmhcb73l4er38iyk?svg=true)](https://ci.appveyor.com/project/melanore/testapp)

Swagger UI for api explorer and docs - https://[env]/index.html
StackExchange miniprofiler for endpoints - https://[env]/profiler/results

Test app implementation of rest api on aspnet core. I have several concerns about architecture.

  - Customer PK constraint 
  - Repository+Uow pattern
  - Unit tests

### Customer PK constraint 

Customer PK constraint is { CustomerId, Name }, while Name is mutable. It is a very bad practice to have a mutable PK, especially in complex domain, especially while using EF. Change of column that is part of constraint will cause reindexing of FKs targeting given record, and will mess up with EF object identity. Custom migration code for EF can fool it, but will cause some unexpected behavior.

More on topic
- https://www.sqlservercentral.com/Forums/Topic440927-373-1.aspx
- https://social.msdn.microsoft.com/Forums/en-US/12fd7e56-2f4d-49e2-ab27-7cfcca6b3e17/entity-framework-updating-composite-primary-key?forum=adodotnetentityframework

### Repository+Uow pattern

Repository and Uow pattern came from  desktop world, and was ment do abstract out EF code from business logic, and allow testability. The reality is - repositories, especially generic repositories are bad practice in web. 

- LINQ is code and data, comming from functional background. Maps, groups, filters and reducers are same as loops, conditional statements and other primitives. You are not trying to hide usage of for loop behind another abstraction, right?
- DbContext is **already** implementing repository+uow pattern, and having abstraction over abstraction on abstraction is just overcomplication.
- Uow is statefull pattern, - state in web is a bad practice. Modular composition should be achieved by small reuseable pieces of logic, I'm totally into stateless services (aka functions) for handling business domain rules, processing and persisting data.
- If project is using EF, - it's typically better to let DbContext spread in DAL service layer. Also for simple or performance critical sections - there is literally zero benefit to using EF instead of Dapper or another micro ORM. Right DbContext usage for persisting can be also enforced by service separator injection into ioc, that will track SaveChanges calls, and not allow different service layers during single request call SaveChanges at same time.

More on topic
- https://www.infoworld.com/article/3117713/application-development/design-patterns-that-i-often-avoid-repository-pattern.html
- https://www.thereformedprogrammer.net/is-the-repository-pattern-useful-with-entity-framework-core/
- https://medium.com/@hoagsie/youre-all-doing-entity-framework-wrong-ea0c40e20502
- https://softwareengineering.stackexchange.com/questions/313188/if-repository-pattern-is-overkill-for-modern-orms-ef-nhibernate-what-is-a-be
- http://georgemauer.net/2018/06/05/whats-wrong-with-repository.html

### Unit test

Unit testing and % of sln test coverage doesn't guarantee good bullet proof tested project. If you have business rules to enforce - better way IMO is to enforce given rules by compiler and language itself. This is where F# shines in .NET - it will provide much more strongly typing with rules encoded into types used in code. Rule engines written in strongly type functional language + DDD provide, and property based testing provide much safer codebase. https://fsharpforfunandprofit.com/posts/property-based-testing/

Also, integration tests with test data, that reflects real test case input, covering solution artefacts are giving much more benefits then mocking. Typically, I would go for parallel integration tests around web layer, with json|xml test data files, and database snapshots for fast test setup/cleanup.
- https://blog.johnnyreilly.com/2016/09/integration-tests-with-sql-server.html
- https://charleskorn.com/2016/03/29/faster-database-testing-with-snapshots/
- https://lostechies.com/jimmybogard/2012/10/18/isolating-database-data-in-integration-tests/
