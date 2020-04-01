# T-SQL for the complete false beginners

Digital transformation is imperative for all businesses, from the small to the enterprise. Digital is not only about technology but also data. Data means databases; databases means SQL.

> **SQL** (Structured Query Language) is the primary language responsible for managing **data** held in a relational database management system (RDBMS), or for stream processing in a relational data stream management system (RDSMS).
>
> Simply put, **SQL** is the language you use to interact with a **database**.

> We live in an era where data is the most valuable asset and it’s being put at heart of every decision making process.

[TablePlus]: https://tableplus.com/blog/2018/08/why-sql-is-the-most-important-skill-to-learn.html	"Why is SQL the most important skill to learn?"

I have been programming in TSQL and C# for more than a decade. In my early software development career I would prefer to write all data query logic in TSQL, encapsulated in stored procedures. One of the reason is that you can easily analyze and improve your SQL query performance thanks to the various SQL performance tools available in SQL editors such as SQL Server Management Studio (SSMS). In those days, I would not favor ORM (Object Relational Mapping) until a few years later when I started to notice the hype of ORM.  Most of the ORM frameworks are getting more intelligent on generating improved performant queries. So, I started with ORM and soon I found that LINQ queries were somewhat difficult than the TSQL queries I used to write. To keep up with ORM/LINQ I started to convert the most common SQL queries to LINQ queries. And now, I would like to share those notes with you.

In this post I will try to refresh the SQL sets and joins knowledge with a sample temporary data set and will demonstrate for each SQL statement the equivalent LINQ query in C# (.NET).

Let's start with creating two simple temporary tables using the IF EXISTS optional clause that is introduced in the DROP statement of MS SQL Server 2016. 

```mssql
--use this DROP table SQL statement in MS SQL Server version prior to MS SQL Server 2016
--IF OBJECT_ID('tempdb.dbo.#Brands', 'U') IS NOT NULL
--BEGIN
--    DROP TABLE #Brands
--END

DROP TABLE IF EXISTS #Brands
CREATE TABLE #Brands
(
	BrandId	INT IDENTITY(1,1), 
	Brand	VARCHAR(100)
)

DROP TABLE IF EXISTS #Models
CREATE TABLE #Models
(
	ModelId		INT  IDENTITY(1,1), 
	ModelName	VARCHAR(100),
	BrandId		INT
)
```

The #Brands table will hold vehicle brands and the #Models table will be used to store the vehicle models. Here's the SQL to add some data in these two temporary tables using Table Value Constructor (TVCs, available since SQL Server 2008).

```mssql
INSERT INTO #Brands (Brand)  
VALUES	('Abarth'), ('Acura'), ('Alfa Romeo'), ('Aston Martin'), ('Audi'), 
		('Bentley'), ('BMW'), 
		('Cadillac'), ('Chevrolet'), ('Chrysler'), ('Citroen'),
		('Dacia'), ('Dodge'),
		('Ferrari'), ('Fiat'), ('Ford'),
		('GMC'), 
		('Honda'), ('Hummer'), ('Hyundai')

INSERT INTO #Models (ModelName, BrandId)  
VALUES	('124 Spider', 1), ('595 C Competizione', 1), ('595 Competizione', 1), ('595 Turismo', 1),
		('ILX Hybrid', 2), ('ILX Premium', 2), ('ILX Technology', 2), ('ILX Technology Plus', 2), 
		('147 Ducati Corse', 3), ('4C Convertible', 3), ('4C Coupe', 3), 
		('Cygnet', 4), ('DB11', 4), ('DB9', 4),
		('A1 Ambition', 5), ('A1 Sportback', 5), ('A3 Attraction', 5),
		('Bentayga', 6), ('Continental Flying Spur', 6), 
		('1-Series 118d', 7), ('1-Series 118d M Sport', 7), ('1-Series 118i', 7),
		('4Runner Limited', NULL), ('Auris Dynamic', NULL)

--SELECT * FROM #Brands
--SELECT * FROM #Models
```

Now, let's see the usage of some DML (Data Manipulation Language) SQL clauses to combine data.

### Joins

As the name implies, a join is used to join or combine two sets of data based on some condition (one or more joining fields or columns). 

#### Inner join

The inner join is the most basic join type. There are two kinds of join namely the equi join and non-equi join. The equi inner join always uses the equals sign (=) to join two data sets whereas the non-equi join uses a non equal sign (anything else than =) to join data sets.

In the below example, the first SQL query (equi inner join) makes sense because it returns all the vehicles of which the brand has a model, brands without a model are omitted. The second SQL query returns all the combination of brand and model except the brand-model combination that are stored in the models table (the models without a brand is not returned as well). 

```mssql
--INNER JOIN (EQUI JOIN)
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
INNER JOIN #Models m
ON b.BrandId = m.BrandId

--INNER JOIN (NON EQUI JOIN)
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
INNER JOIN #Models m
ON b.BrandId != m.BrandId
```



#### Cross join

The cross join returns all possible combinations of brand and model, so the combinations of brand and model returned by the equi inner join and non-equi inner join are actually a subset of the cross join's result set. 

Please note that there are no ON clause for a cross join because it creates a combination of every row from two data sets (tables), so no column-based condition required. 

```mssql
--CROSS JOIN, the Cartesian product
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
CROSS JOIN #Models m 
```

Inner join (equi) using C# LINQ:

```c#
//INNER JOIN (EQUI JOIN)
//Query syntax
var equi_inner_join_query = from b in brands
                            join m in models on b.BrandId equals m.BrandId
                            select new { b.BrandName, m.ModelName, Vehicle = $"{b.BrandName} {m.ModelName}" };

//Method syntax
var equi_inner_join_query = brands.Join(models,
                            b => b.BrandId,
                            m => m.BrandId,
                            (b, m) => new { b.BrandName, m.ModelName, Vehicle = $"{b.BrandName} {m.ModelName}" });

equi_inner_join_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
```

Since there's no "not equal" keyword in C# LINQ, we have to create a workaround for the non-equi LINQ query. One way is to use a combination of the cross join and the inner join LINQ query. Let's check the cross join first.

Cross join using C# LINQ:

```c#
//CROSS JOIN, the Cartesian product
//Query syntax
var cross_join_query = from b in brands
                       from m in models
                       select new { b.BrandName, m.ModelName, Vehicle = $"{b.BrandName} {m.ModelName}" };


//Method syntax - using SelectMany
var cross_join_query = brands
                       .SelectMany(m => models,
                       (b, m) => new
                       { b.BrandName, m.ModelName, Vehicle = $"{b.BrandName} {m.ModelName}" });

////Method syntax - using Join
var cross_join_query = brands
                       .Join(models,
                       b => true,
                       m => true,
                       (b, m) => new
                       { b.BrandName, m.ModelName, Vehicle = $"{b.BrandName} {m.ModelName}" });


cross_join_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
```

Inner join (**non-equi**) using C# LINQ:

```C#
//INNER JOIN (NON EQUI JOIN)
//Query syntax
var non_equi_inner_join_query = from c in cross_join_query
                                where !equi_inner_join_query.Any(i => i.BrandName == c.BrandName && i.ModelName == c.ModelName)
                                && !models.Where(m => !m.BrandId.HasValue).Any(i => i.ModelName == c.ModelName)
                                select new { c.BrandName, c.ModelName, c.Vehicle };


//Method syntax
var non_equi_inner_join_query = cross_join_query
                                .Where(c => !equi_inner_join_query.Any(i => i.BrandName == c.BrandName && i.ModelName == c.ModelName)
                                && !models.Where(m => !m.BrandId.HasValue).Any(i => i.ModelName == c.ModelName));

non_equi_inner_join_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
```



#### Outer joins

The inner join is all about the matching/matched rows whereas the outer joins can include unmatched rows as well. The following outer joins are demonstrated. 

```mssql
--LEFT OUTER JOIN
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
LEFT OUTER JOIN #Models m
ON b.BrandId = m.BrandId

--RIGHT OUTER JOIN
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
RIGHT OUTER JOIN #Models m
ON b.BrandId = m.BrandId
```

The first SQL query (left outer join) returns all rows from Brands table regardless of whatever condition (matched or unmatched rows in Models table) since the Brands table is on the left side of the join. So it returns matched (brand associated with a model) and unmatched (brand without a model) rows.  

The second SQL query (right outer query) returns all models even the models that have no brand associated with. 

Here are the two outer joins in C# LINQ:

```C#
//LEFT OUTER JOIN
//Query syntax
var left_outer_join = from b in brands
                      join m in models on b.BrandId equals m.BrandId into vehicles
                      from v in vehicles.DefaultIfEmpty()
                      select (b.BrandName, ModelName: v != null
                                ? v.ModelName
                                : string.Empty, Vehicle: v != null ? $"{b.BrandName} {v.ModelName}" : $"{b.BrandName}");

//Method syntax
var left_outer_join = brands.GroupJoin(
                      models,
                      b => b.BrandId,
                      m => m.BrandId,
                      (b, m) => new { Brand = b, vehicles = m })
                      .SelectMany(
                      v => v.vehicles.DefaultIfEmpty(),
                      (b, m) => (b.Brand.BrandName, ModelName: m != null
                                   ? m.ModelName
                                   : string.Empty, Vehicle: m != null ? $"{b.Brand.BrandName} {m.ModelName}" : $"{b.Brand.BrandName}"));


left_outer_join.ToList().ForEach(v => Console.WriteLine(v.Vehicle));


//RIGHT OUTER JOIN
//Query syntax
var right_outer_join = from m in models
                       join b in brands
                       on m.BrandId equals b.BrandId into vehicles
                       from v in vehicles.DefaultIfEmpty()
                       select (BrandName: v != null
                                ? v?.BrandName
                                : string.Empty, m.ModelName, Vehicle: v != null ? $"{v?.BrandName} {m.ModelName}" : $"{m.ModelName}");

//Method syntax
var right_outer_join = models.GroupJoin(
                       brands,
                       m => m.BrandId,
                       b => b.BrandId,
                       (m, b) => new { Model = m, vehicles = b })
                       .SelectMany(
                       v => v.vehicles.DefaultIfEmpty(),
                       (m, b) => (b?.BrandName, m.Model.ModelName, Vehicle: m != null
                                    ? $"{b?.BrandName} {m.Model.ModelName}"
                                    : $"{m.Model.ModelName}"));

right_outer_join.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
```



##### Outer join with exclusion

The following outer joins are used to display only the unmatched rows. The first SQL query returns all brands that have no model associated with. And the second SQL query returns all models that have no brands.

```mssql
--LEFT OUTER JOIN with exclusion
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
LEFT OUTER JOIN #Models m
ON b.BrandId = m.BrandId
WHERE m.BrandId IS NULL

--RIGHT OUTER JOIN with exclusion
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
RIGHT OUTER JOIN #Models m
ON b.BrandId = m.BrandId
WHERE b.BrandId IS NULL
```

C# LINQ outer joins with exclusions:

```C#
//LEFT OUTER JOIN with exclusion
//Query syntax
var left_outer_join_with_exclusion = from m in left_outer_join
                                     where string.IsNullOrEmpty(m.ModelName)
                                     select m;

//Method syntax
var left_outer_join_with_exclusion = left_outer_join.Where(m => string.IsNullOrEmpty(m.ModelName));

left_outer_join_with_exclusion.ToList().ForEach(v => Console.WriteLine(v.Vehicle));

//RIGHT OUTER JOIN with exclusion
//Query syntax
var right_outer_join_with_exclusion = from b in right_outer_join
                                      where string.IsNullOrEmpty(b.BrandName)
                                      select b;

//Method syntax
var right_outer_join_with_exclusion = right_outer_join.Where(b => string.IsNullOrEmpty(b.BrandName));

right_outer_join_with_exclusion.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
```

What if you want to display all unmatched rows from both tables in one result set (think of a left outer join combined with a right outer join). You can easily accomplish this using a full outer join with exclusion. Or perhaps, you could use the union all clause to combine the left outer join with the right outer join SQL query.

```mssql
--FULL OUTER JOIN with exclusion
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
FULL OUTER JOIN #Models m
ON b.BrandId = m.BrandId
WHERE m.BrandId IS NULL OR b.BrandId IS NULL
```

A full outer join with exclusion in C# LINQ:

```C#
//FULL OUTER JOIN with exclusion | using union
var full_join_query_with_exclusion_query = left_outer_join_with_exclusion_query.Union(right_outer_join_with_exclusion_query);

//FULL OUTER JOIN with exclusion | using except
var full_join_query_with_exclusion_query = full_join_query.Except(equi_inner_join_query);

full_join_query_with_exclusion_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
```

What if you want to display all the unmatched and matched rows in one results set. Such a result set is returned by the full outer join. The full outer join is a combination of the inner, left outer and right outer joins.  

```mssql
--FULL OUTER JOIN
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
FULL OUTER JOIN #Models m
ON b.BrandId = m.BrandId
```

In C# LINQ, we can simply use the union to combine the left outer join query with the right outer join query:

```C#
//FULL OUTER JOIN
var full_join_query = left_outer_join_query.Union(right_outer_join_query);

full_join_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
```



#### Semi joins vs anti joins

A **semi**-**join** between two tables returns rows from the first table where one or more matches are found in the second table (see the 'in' operator). The anti-(semi) join works in the opposite direction, it returns the rows from the first table that are not found in the second table (see the 'not in' operator).

> **Semi join** is one of a few operators in relational algebra that does not have representation in T-SQL language.
>
> [SEMI JOINS, ANTI-JOINS AND NULLS IN SQL SERVER]: https://sqlchitchat.com/sqldev/tsql/semi-joins-in-sql-server/	"SEMI JOINS, ANTI-JOINS AND NULLS IN SQL SERVER"

##### Semi joins

Suppose we are only interested in the matching brands. In that case we could make use of semi joins. 

Notice that when you use the IN operator you are allowed to return only one column from the sub-query. Therefore, the EXISTS operator is very handy when you want to evaluate on multiple columns from the sub-query. 

```mssql
--SEMI JOIN
SELECT Brand AS MatchingBrands 
FROM #Brands b 
WHERE EXISTS (SELECT 1 FROM #Models m WHERE b.BrandId = m.BrandId) 

--IN 
SELECT Brand AS MatchingBrands 
FROM #Brands b 
WHERE BrandId IN (SELECT BrandId FROM #Models) 
```

Here is the C# LINQ query using the IN operator:

```C#
//IN
//Query syntax
var in_query = from b in brands
               where (from m in equi_inner_join_query select m.BrandName)
               .Contains(b.BrandName)
               select b.BrandName;

//Method syntax
var in_query = brands.Select(b => b.BrandName)
               .Where(b => equi_inner_join_query.Select(i => i.BrandName).Contains(b));

in_query.ToList().ForEach(Console.WriteLine);
```

The semi join (EXISTS) can be represented in C# LINQ with an Enumerable.**Intersect** as well. 

##### Anti joins

The so-called anti joins can be used to return the non matching brands. It is the opposite of the semi joins. 

```mssql
--ANTI SEMI JOIN
SELECT Brand AS NonMatchingBrands 
FROM #Brands b 
WHERE NOT EXISTS (SELECT 1 FROM #Models m WHERE b.BrandId = m.BrandId) 

--NOT IN 
SELECT Brand AS NonMatchingBrands 
FROM #Brands b 
--WHERE BrandId NOT IN (SELECT BrandId FROM #Models) -> this won't work because of NULL brand id's in the #Models table
WHERE b.BrandId NOT IN (SELECT DISTINCT BrandId FROM #Models WHERE BrandId IS NOT NULL) 
```

You can read more about semi join (U-SQL) [here](https://docs.microsoft.com/en-us/u-sql/statements-and-expressions/select/from/joins/semijoin).

Here is the C# LINQ query using the NOT IN (**!**) operator: 

```C#
//NOT IN
//Query syntax
var not_in_query = from b in brands
                   where !(from m in equi_inner_join_query select m.BrandName).Contains(b.BrandName)
                   select b.BrandName;

//Method syntax
var not_in_query = brands.Select(b => b.BrandName).Where(b => !equi_inner_join_query.Select(i => i.BrandName).Contains(b));
            
not_in_query.ToList().ForEach(Console.WriteLine);
```

The anti-semi join (NOT EXISTS) can be represented in C# LINQ with an Enumerable.**Except**. 

### The apply operator

The apply operator can return the same result set that is returned by a join. For example the inner join SQL query could be rewritten using the cross apply operator. And the left outer join SQL query could be rewritten using the outer apply operator.  

```mssql
--CROSS APPLY (used with correlated sub-query, can also be used with table valued function)
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
CROSS APPLY (SELECT ModelName FROM #Models m WHERE b.BrandId = m.BrandId) m

--OUTER APPLY (used with correlated sub-query, can also be used with table valued function)
SELECT Brand, ModelName, CONCAT(Brand, ' ', ModelName) AS Vehicle 
FROM #Brands b 
OUTER APPLY (SELECT ModelName FROM #Models m WHERE b.BrandId = m.BrandId) m
```

The apply operator, that is introduced in SQL Server 2005, is there to be used with a table-valued expression. It evaluates a table-valued expression for each row of the left table. 

Here is an example of a SQL query with the apply operator and dynamic TOP clause. Since SQL Server 2005 it's possible to use a variable in the TOP clause. 

> In a SELECT statement, always use an ORDER BY clause with the TOP clause. Because, it's the only way to predictably indicate which rows are affected by TOP. (Best practice - https://docs.microsoft.com)

```mssql
SELECT  *
FROM    table1
CROSS APPLY
(
    SELECT  	TOP (table1.rowcount) *
    FROM    	table2
    ORDER BY 	id
) t2
```

[EXPLAIN EXTENDED]: https://explainextended.com/2009/07/16/inner-join-vs-cross-apply/	"INNER JOIN vs. CROSS APPLY"



### **Set** operators

Set operators allows you to combine data sets (multiple select statements) into one result set (one combined data set). All the participating data sets must meet some rules. The rules can be found [here](https://sql-programmers.com/set-operators-in-sql-server-union-union-all-intersect-except).

Note: The column names used in the result set are taken from the first (select) query.

There are four set operators namely:

- UNION: combines multiple data sets in one, without duplicated records.
- UNION ALL: same as union but returns duplicated records as well.
- EXCEPT (Oracle's variant is MINUS): returns the records that are only in the first data set and not in the second data set. Except operator excludes the records of the second data set from the first data set. 
- INTERSECT: returns only the records that are common in all the participating data sets.

In the below example we are using the except operator to find the non matching brands (brands without a model). The same result could be achieved by using an outer join with exclusion or the anti join namely NOT IN.

```mssql
--EXCEPT
SELECT Brand AS NonMatchingBrands --Brands without a model
FROM #Brands b 
EXCEPT
SELECT Brand
FROM #Brands b 
INNER JOIN #Models m
ON b.BrandId = m.BrandId

```

Except in C# LINQ query to find the brands that have no model:

```C#
//EXCEPT
var except_query = brands.Select(b => b.BrandName).Except(equi_inner_join_query.Select(i => i.BrandName));
except_query.ToList().ForEach(Console.WriteLine);
```

Let's find the brands with a model using the intersect operator. The same result could be achieved by the inner join or IN operator.

```mssql
--INTERSECT
SELECT Brand AS MatchingBrands -- Brands with a model 
FROM #Brands b 
INTERSECT
SELECT Brand
FROM #Brands b 
INNER JOIN #Models m
ON b.BrandId = m.BrandId
```

Intersect C# LINQ query to find the brands that have one or more model(s). I know, this query does not make sense because you could have used the ''equi_inner_join_query'' simply.

```C#
var intersect_query = brands.Select(b => b.BrandName).Intersect(equi_inner_join_query.Select(i => i.BrandName));
intersect_query.ToList().ForEach(Console.WriteLine);
```

Now, we can combine these two results with the union. We could have used 'union all' but in this sample it would return the same result set since both the participating data sets are unique (having no common records). 

```mssql
--UNION
SELECT Brand, 'YES' AS IsMatching
FROM #Brands b 
WHERE EXISTS (SELECT 1 FROM #Models m WHERE b.BrandId = m.BrandId) 
UNION
SELECT Brand, 'NO' 
FROM #Brands b 
WHERE NOT EXISTS (SELECT 1 FROM #Models m WHERE b.BrandId = m.BrandId) 

```

Union C# LINQ query to merge two data sets into one:

```C#
//UNION
var union_query = brands.Select(b => b.BrandName).Union(equi_inner_join_query.Select(i => i.BrandName));
union_query.ToList().ForEach(Console.WriteLine);
```

I used the following software to write this post:

- Typora
- SSMS
- SQL Server 2016
