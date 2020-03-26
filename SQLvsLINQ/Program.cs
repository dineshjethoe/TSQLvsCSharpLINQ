using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLvsLINQ
{

    class Program
    {
        static int brandId = 1;
        private static readonly List<Brand> brands = new List<Brand> {
            new Brand { BrandId = brandId++, BrandName = "Abarth" },
            new Brand { BrandId = brandId++, BrandName = "Acura" },
            new Brand { BrandId = brandId++, BrandName = "Alfa Romeo" },
            new Brand { BrandId = brandId++, BrandName = "Aston Martin" },
            new Brand { BrandId = brandId++, BrandName = "Audi" },
            new Brand { BrandId = brandId++, BrandName = "Bentley" },
            new Brand { BrandId = brandId++, BrandName = "BMW" },
            new Brand { BrandId = brandId++, BrandName = "Cadillac" },
            new Brand { BrandId = brandId++, BrandName = "Chevrolet" },
            new Brand { BrandId = brandId++, BrandName = "Chrysler" },
            new Brand { BrandId = brandId++, BrandName = "Citroen" },
            new Brand { BrandId = brandId++, BrandName = "Dacia" },
            new Brand { BrandId = brandId++, BrandName = "Dodge" },
            new Brand { BrandId = brandId++, BrandName = "Ferrari" },
            new Brand { BrandId = brandId++, BrandName = "Fiat" },
            new Brand { BrandId = brandId++, BrandName = "Ford" },
            new Brand { BrandId = brandId++, BrandName = "GMC" },
            new Brand { BrandId = brandId++, BrandName = "Honda" },
            new Brand { BrandId = brandId++, BrandName = "Hummer" },
            new Brand { BrandId = brandId++, BrandName = "Hyundai" },
        };

        static int modelId = 1;
        private static readonly List<Model> models = new List<Model> {
            new Model { ModelId = modelId++, ModelName = "124 Spider", BrandId = 1 },
            new Model { ModelId = modelId++, ModelName = "595 C Competizione", BrandId = 1 },
            new Model { ModelId = modelId++, ModelName = "595 Competizione", BrandId = 1 },
            new Model { ModelId = modelId++, ModelName = "595 Turismo", BrandId = 1 },
            new Model { ModelId = modelId++, ModelName = "ILX Hybrid", BrandId = 2 },
            new Model { ModelId = modelId++, ModelName = "ILX Premium", BrandId = 2 },
            new Model { ModelId = modelId++, ModelName = "ILX Technology", BrandId = 2 },
            new Model { ModelId = modelId++, ModelName = "ILX Technology Plus", BrandId = 2 },
            new Model { ModelId = modelId++, ModelName = "147 Ducati Corse", BrandId = 3 },
            new Model { ModelId = modelId++, ModelName = "4C Convertible", BrandId = 3 },
            new Model { ModelId = modelId++, ModelName = "4C Coupe", BrandId = 3 },
            new Model { ModelId = modelId++, ModelName = "Cygnet", BrandId = 4 },
            new Model { ModelId = modelId++, ModelName = "AbaDB11rth", BrandId = 4 },
            new Model { ModelId = modelId++, ModelName = "DB9", BrandId = 4 },
            new Model { ModelId = modelId++, ModelName = "A1 Ambition", BrandId = 5 },
            new Model { ModelId = modelId++, ModelName = "A1 Sportback", BrandId = 5 },
            new Model { ModelId = modelId++, ModelName = "A3 Attraction", BrandId = 5 },
            new Model { ModelId = modelId++, ModelName = "Bentayga", BrandId = 6 },
            new Model { ModelId = modelId++, ModelName = "Continental Flying Spur", BrandId = 6 },
            new Model { ModelId = modelId++, ModelName = "1-Series 118d", BrandId = 7 },
            new Model { ModelId = modelId++, ModelName = "1-Series 118d M Sport", BrandId = 7 },
            new Model { ModelId = modelId++, ModelName = "1-Series 118i", BrandId = 7 },
            new Model { ModelId = modelId++, ModelName = "4Runner Limited", BrandId = null },
            new Model { ModelId = modelId++, ModelName = "Auris Dynamic", BrandId = null },
        };

        static void Main(string[] args)
        {

            //brands.ToList().ForEach(b => Console.WriteLine(b));
            //Console.WriteLine(Environment.NewLine);
            //models.ToList().ForEach(m => Console.WriteLine(m));

            #region INNER JOIN (EQUI JOIN)
            Console.WriteLine($"{Environment.NewLine}INNER JOIN (EQUI JOIN):");
            //Query syntax
            var equi_inner_join_query = from b in brands
                                        join m in models on b.BrandId equals m.BrandId
                                        select (b.BrandName, m.ModelName, Vehicle: $"{b.BrandName} {m.ModelName}");

            //Method syntax
            //var equi_inner_join_query = brands.Join(models,
            //                            b => b.BrandId,
            //                            m => m.BrandId,
            //                            (b, m) => new { b.BrandName, m.ModelName, Vehicle = $"{b.BrandName} {m.ModelName}" });

            equi_inner_join_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
            #endregion

            #region CROSS JOIN, the Cartesian product
            Console.WriteLine($"{Environment.NewLine}CROSS JOIN, the Cartesian product:");
            //Query syntax
            var cross_join_query = from b in brands
                                   from m in models
                                   select new { b.BrandName, m.ModelName, Vehicle = $"{b.BrandName} {m.ModelName}" };


            //Method syntax - using SelectMany
            //var cross_join_query = brands
            //                       .SelectMany(m => models,
            //                       (b, m) => new
            //                       { b.BrandName, m.ModelName, Vehicle = $"{b.BrandName} {m.ModelName}" });

            //Method syntax - using Join
            //var cross_join_query = brands
            //                       .Join(models,
            //                       b => true,
            //                       m => true,
            //                       (b, m) => new
            //                       { b.BrandName, m.ModelName, Vehicle = $"{b.BrandName} {m.ModelName}" });


            cross_join_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
            #endregion

            #region INNER JOIN (NON EQUI JOIN)
            Console.WriteLine($"{Environment.NewLine}INNER JOIN (NON EQUI JOIN):");
            //Query syntax
            var non_equi_inner_join_query = from c in cross_join_query
                                            where !equi_inner_join_query.Any(i => i.BrandName == c.BrandName && i.ModelName == c.ModelName)
                                            && !models.Where(m => !m.BrandId.HasValue).Any(i => i.ModelName == c.ModelName)
                                            select new { c.BrandName, c.ModelName, c.Vehicle };


            //Method syntax
            //var non_equi_inner_join_query = cross_join_query
            //                                .Where(c => !equi_inner_join_query.Any(i => i.BrandName == c.BrandName && i.ModelName == c.ModelName)
            //                                && !models.Where(m => !m.BrandId.HasValue).Any(i => i.ModelName == c.ModelName));

            non_equi_inner_join_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
            #endregion

            #region SETS
            //EXCEPT
            Console.WriteLine($"{Environment.NewLine}EXCEPT:");
            var except_query = brands.Select(b => b.BrandName).Except(equi_inner_join_query.Select(i => i.BrandName));
            except_query.ToList().ForEach(Console.WriteLine);

            //INTERSECT
            Console.WriteLine($"{Environment.NewLine}INTERSECT:");
            var intersect_query = brands.Select(b => b.BrandName).Intersect(equi_inner_join_query.Select(i => i.BrandName));
            intersect_query.ToList().ForEach(Console.WriteLine);

            //UNION
            Console.WriteLine($"{Environment.NewLine}UNION:");
            var union_query = brands.Select(b => b.BrandName).Union(equi_inner_join_query.Select(i => i.BrandName));
            union_query.ToList().ForEach(Console.WriteLine);
            #endregion

            #region IN & NOT IN
            Console.WriteLine($"{Environment.NewLine}IN:");
            //Query syntax
            var in_query = from b in brands
                           where (from m in equi_inner_join_query select m.BrandName).Contains(b.BrandName)
                           select b.BrandName;

            //Method syntax
            //var in_query = brands.Select(b => b.BrandName).Where(b => equi_inner_join_query.Select(i => i.BrandName).Contains(b));

            in_query.ToList().ForEach(Console.WriteLine);

            //NOT IN
            Console.WriteLine($"{Environment.NewLine}NOT IN:");
            //Query syntax
            var not_in_query = from b in brands
                               where !(from m in equi_inner_join_query select m.BrandName).Contains(b.BrandName)
                               select b.BrandName;

            //Method syntax
            //var not_in_query = brands.Select(b => b.BrandName).Where(b => !equi_inner_join_query.Select(i => i.BrandName).Contains(b));

            not_in_query.ToList().ForEach(Console.WriteLine);
            #endregion

            #region LEFT OUTER JOIN
            Console.WriteLine($"{Environment.NewLine}LEFT OUTER JOIN:");
            //Query syntax
            var left_outer_join_query = from b in brands
                                  join m in models on b.BrandId equals m.BrandId into vehicles
                                  from v in vehicles.DefaultIfEmpty()
                                  select (b.BrandName, ModelName: v != null
                                    ? v.ModelName
                                    : string.Empty, Vehicle: v != null ? $"{b.BrandName} {v.ModelName}" : $"{b.BrandName}");

            //Method syntax
            //var left_outer_join_query = brands.GroupJoin(
            //                      models,
            //                      b => b.BrandId,
            //                      m => m.BrandId,
            //                      (b, m) => new { Brand = b, vehicles = m })
            //                      .SelectMany(
            //                      v => v.vehicles.DefaultIfEmpty(),
            //                      (b, m) => (b.Brand.BrandName, ModelName: m != null
            //                        ? m.ModelName
            //                        : string.Empty, Vehicle: m != null ? $"{b.Brand.BrandName} {m.ModelName}" : $"{b.Brand.BrandName}"));


            left_outer_join_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
            #endregion

            #region RIGHT OUTER JOIN
            Console.WriteLine($"{Environment.NewLine}RIGHT OUTER JOIN:");
            //Query syntax
            var right_outer_join_query = from m in models
                                   join b in brands
                                   on m.BrandId equals b.BrandId into vehicles
                                   from v in vehicles.DefaultIfEmpty()
                                   select (BrandName: v != null
                                       ? v?.BrandName
                                       : string.Empty, m.ModelName, Vehicle: v != null ? $"{v?.BrandName} {m.ModelName}" : $"{m.ModelName}");

            //Method syntax
            //var right_outer_join_query = models.GroupJoin(
            //                      brands,
            //                      m => m.BrandId,
            //                      b => b.BrandId,
            //                      (m, b) => new { Model = m, vehicles = b })s
            //                      .SelectMany(
            //                      v => v.vehicles.DefaultIfEmpty(),
            //                      (m, b) => (b?.BrandName, m.Model.ModelName, Vehicle: m != null
            //                        ? $"{b?.BrandName} {m.Model.ModelName}"
            //                        : $"{m.Model.ModelName}"));

            right_outer_join_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
            #endregion

            #region LEFT OUTER JOIN with exclusion
            Console.WriteLine($"{Environment.NewLine}LEFT OUTER JOIN with exclusion:");
            //Query syntax
            var left_outer_join_with_exclusion_query = from m in left_outer_join_query
                                                       where string.IsNullOrEmpty(m.ModelName)
                                                       select m;

            //Method syntax
            //var left_outer_join_with_exclusion_query = left_outer_join_query.Where(m => string.IsNullOrEmpty(m.ModelName));

            left_outer_join_with_exclusion_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
            #endregion

            #region RIGHT OUTER JOIN with exclusion
            Console.WriteLine($"{Environment.NewLine}RIGHT OUTER JOIN with exclusion:");
            //Query syntax
            var right_outer_join_with_exclusion_query = from b in right_outer_join_query
                                                        where string.IsNullOrEmpty(b.BrandName)
                                                        select b;

            //Method syntax
            //var right_outer_join_with_exclusion_query = right_outer_join_query.Where(b => string.IsNullOrEmpty(b.BrandName));

            right_outer_join_with_exclusion_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
            #endregion

            #region FULL OUTER JOIN (with and without exclusion)
            Console.WriteLine($"{Environment.NewLine}FULL OUTER JOIN:");
            var full_join_query = left_outer_join_query.Union(right_outer_join_query);

            full_join_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));

            //FULL OUTER JOIN with exclusion | using union
            var full_join_query_with_exclusion_query = left_outer_join_with_exclusion_query.Union(right_outer_join_with_exclusion_query);

            //FULL OUTER JOIN with exclusion | using except
            //var full_join_query_with_exclusion_query = full_join_query.Except(equi_inner_join_query);

            full_join_query_with_exclusion_query.ToList().ForEach(v => Console.WriteLine(v.Vehicle));
            #endregion

            Console.ReadKey();
        }
    }
}
