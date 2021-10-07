using System;
using System.Collections.Generic;
using DatabaseParser.Base;
using DatabaseParser.ExpressionParser;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Xunit;

namespace TestProject1
{

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool HaveChildren { get; set; }
    }

    public class Pet
    {
        public string Name { get; set; }
        public int Age;
        public string Address;
    }

    public class Dog : Pet
    {

    }
    public class PersonRepository : BaseRepository<Person>, IPersonRepository
    {
        public PersonRepository() : base(DatabaseType.SqlServer)
        {

        }
        public void ExecuteDelete()
        {

        }
    }

    public interface IPersonRepository : IRepository<Person>
    {
        void ExecuteDelete();
    }
    public class UnitTestExpressionTreeVisitor
    {
        [Fact]
        public void TestSelect()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Select(it => it).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);
        }

        [Fact]
        public void TestSelect2()
        {
            var personRepository = new PersonRepository();
            var r2 = personRepository.Select(it => it.Name).ToList();
            var r2MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name] FROM [Person] As [p0]", r2MiddleResult.Sql);
            Assert.Empty(r2MiddleResult.SqlParameters);
        }


        [Fact]
        public void TestSelect3()
        {
            var personRepository = new PersonRepository();
            var r3 = personRepository.Select(it => new { it.Name, Address = "福建" }).ToList();
            var r3MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], '福建' As [Address] FROM [Person] As [p0]", r3MiddleResult.Sql);
            Assert.Empty(r3MiddleResult.SqlParameters);
        }

        [Fact]
        public void TestSelect4()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Name = "Dog" };
            var r4 = personRepository.Select(it => new { it.Name, Address = pet.Name }).ToList();
            var r4MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], 'Dog' As [Address] FROM [Person] As [p0]", r4MiddleResult.Sql);
            Assert.Empty(r4MiddleResult.SqlParameters);


        }

        [Fact]
        public void TestWhere2()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.HaveChildren && it.Name == "hzp" && it.Age == 15).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ( ([p0].[HaveChildren] = @y0 ) AND  ([p0].[Name] = @y1 )  ) AND  ([p0].[Age] = @y2 )  )", r1MiddleResult.Sql);
            Assert.Equal(3, r1MiddleResult.SqlParameters.Count);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[0].Value);
            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[1].Value);
            Assert.Equal("@y2", r1MiddleResult.SqlParameters[2].ParameterName);
            Assert.Equal(15, r1MiddleResult.SqlParameters[2].Value);
        }

        [Fact]
        public void TestWhere3()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.HaveChildren == false && it.Name == "hzp" && it.Age == 15).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ( ([p0].[HaveChildren] = @y0 ) AND  ([p0].[Name] = @y1 )  ) AND  ([p0].[Age] = @y2 )  )", r1MiddleResult.Sql);
            Assert.Equal(3, r1MiddleResult.SqlParameters.Count);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(0, r1MiddleResult.SqlParameters[0].Value);
            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[1].Value);
            Assert.Equal("@y2", r1MiddleResult.SqlParameters[2].ParameterName);
            Assert.Equal(15, r1MiddleResult.SqlParameters[2].Value);
        }

        [Fact]
        public void TestWhere4()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name == "hzp" && it.HaveChildren == false && it.Age == 15).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ( ([p0].[Name] = @y0 ) AND  ([p0].[HaveChildren] = @y1 )  ) AND  ([p0].[Age] = @y2 )  )", r1MiddleResult.Sql);
            Assert.Equal(3, r1MiddleResult.SqlParameters.Count);
            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal(0, r1MiddleResult.SqlParameters[1].Value);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
            Assert.Equal("@y2", r1MiddleResult.SqlParameters[2].ParameterName);
            Assert.Equal(15, r1MiddleResult.SqlParameters[2].Value);
        }

        [Fact]
        public void TestWhere5()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name == "hzp" && it.HaveChildren && it.Age == 15).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ( ([p0].[Name] = @y0 ) AND  ([p0].[HaveChildren] = @y1 )  ) AND  ([p0].[Age] = @y2 )  )", r1MiddleResult.Sql);
            Assert.Equal(3, r1MiddleResult.SqlParameters.Count);
            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[1].Value);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
            Assert.Equal("@y2", r1MiddleResult.SqlParameters[2].ParameterName);
            Assert.Equal(15, r1MiddleResult.SqlParameters[2].Value);
        }

        [Fact]
        public void TestWhere6()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.HaveChildren && it.HaveChildren).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ([p0].[HaveChildren] = @y0 ) AND  ([p0].[HaveChildren] = @y1 )  )", r1MiddleResult.Sql);
            Assert.Equal(2, r1MiddleResult.SqlParameters.Count);
            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[1].Value);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[0].Value);

        }
        [Fact]
        public void TestWhere7()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => !it.HaveChildren).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (NOT  ( ([p0].[HaveChildren] = @y0 ) ) )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[0].Value);
        }
        [Fact]
        public void TestWhere8()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => !!it.HaveChildren).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (NOT  ( (NOT  ( ([p0].[HaveChildren] = @y0 ) ) ) ) )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere9()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.Length > 3).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (LEN([p0].[Name]) > @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(3, r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere10()
        {
            var pet = new Pet() { Name = "小黄" };
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name == pet.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("小黄", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere11()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name == "hzp" && (it.HaveChildren && it.Age == 15)).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ([p0].[Name] = @y0 ) AND  ( ([p0].[HaveChildren] = @y1 ) AND  ([p0].[Age] = @y2 )  )  )", r1MiddleResult.Sql);
            Assert.Equal(3, r1MiddleResult.SqlParameters.Count);
            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[1].Value);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
            Assert.Equal("@y2", r1MiddleResult.SqlParameters[2].ParameterName);
            Assert.Equal(15, r1MiddleResult.SqlParameters[2].Value);
        }

        [Fact]
        public void TestWhere12()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => (it.Name == "hzp" && it.HaveChildren) && it.Age == 15).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ( ([p0].[Name] = @y0 ) AND  ([p0].[HaveChildren] = @y1 )  ) AND  ([p0].[Age] = @y2 )  )", r1MiddleResult.Sql);
            Assert.Equal(3, r1MiddleResult.SqlParameters.Count);
            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[1].Value);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
            Assert.Equal("@y2", r1MiddleResult.SqlParameters[2].ParameterName);
            Assert.Equal(15, r1MiddleResult.SqlParameters[2].Value);
        }

        [Fact]
        public void TestWhere13()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.Contains("hzp")).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] like @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("%hzp%", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere14()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Name = "hzp" };
            var r1 = personRepository.Where(it => it.Name.Contains(pet.Name)).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] like @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("%hzp%", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere15()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.StartsWith("hzp")).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] like @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp%", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere16()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Name = "hzp" };
            var r1 = personRepository.Where(it => it.Name.StartsWith(pet.Name)).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] like @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp%", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere17()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.EndsWith("hzp")).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] like @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("%hzp", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere18()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Name = "hzp" };
            var r1 = personRepository.Where(it => it.Name.EndsWith(pet.Name)).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] like @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("%hzp", r1MiddleResult.SqlParameters[0].Value);
        }
        [Fact]
        public void TestWhere19()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.Trim() == "hzp").ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (TRIM([p0].[Name]) = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere20()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Name = "hzp" };
            var r1 = personRepository.Where(it => it.Name.Trim() == pet.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (TRIM([p0].[Name]) = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }
        [Fact]
        public void TestWhere21()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.TrimStart() == "hzp").ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (LTRIM([p0].[Name]) = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere22()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Name = "hzp" };
            var r1 = personRepository.Where(it => it.Name.TrimStart() == pet.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (LTRIM([p0].[Name]) = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }
        [Fact]
        public void TestWhere23()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.TrimEnd() == "hzp").ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (RTRIM([p0].[Name]) = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere24()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Name = "hzp" };
            var r1 = personRepository.Where(it => it.Name.TrimEnd() == pet.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (RTRIM([p0].[Name]) = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }
        [Fact]
        public void TestWhere25()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.Equals("hzp")).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere26()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Name = "hzp" };
            var r1 = personRepository.Where(it => it.Name.Equals(pet.Name)).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere27()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Address = "ND" };
            var r1 = personRepository.Where(it => it.Name.Equals(pet.Address)).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("ND", r1MiddleResult.SqlParameters[0].Value);
        }


        [Fact]
        public void TestWhere29()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Name = "hzp" };
            var r1 = personRepository.Where(it => it.Name.ToLower() == pet.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (LOWER([p0].[Name]) = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }
        [Fact]
        public void TestWhere30()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.ToLower() == "hzp").ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (LOWER([p0].[Name]) = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere31()
        {
            var personRepository = new PersonRepository();
            var pet = new Pet() { Name = "HZP" };
            var r1 = personRepository.Where(it => it.Name.ToUpper() == pet.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (UPPER([p0].[Name]) = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("HZP", r1MiddleResult.SqlParameters[0].Value);
        }
        [Fact]
        public void TestWhere32()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.ToUpper() == "hzp").ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  (UPPER([p0].[Name]) = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }

        /// <summary>
        /// colloection
        /// </summary>
        [Fact]
        public void TestWhere33()
        {
            var personRepository = new PersonRepository();
            var nameList = new List<string>() { "hzp", "qy" };
            var r1 = personRepository.Where(it => nameList.Contains(it.Name)).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] in @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(nameList, r1MiddleResult.SqlParameters[0].Value);
        }

        /// <summary>
        /// colloection
        /// </summary>
        [Fact]
        public void TestWhere34()
        {
            var personRepository = new PersonRepository();
            var nameList = new string[] { "hzp", "qy" };
            var r1 = personRepository.Where(it => nameList.Contains(it.Name)).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] in @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(nameList, r1MiddleResult.SqlParameters[0].Value);
        }

        /// <summary>
        /// colloection
        /// </summary>
        [Fact]
        public void TestWhere35()
        {
            var personRepository = new PersonRepository();
            var nameList = Enumerable.Range(0, 1001).Select(it => "hzp" + it).ToList();

            var p0 = nameList.Take(500).ToList();
            var p1 = nameList.Skip(500).Take(500).ToList();
            var p2 = nameList.Skip(1000).Take(500).ToList();

            var r1 = personRepository.Where(it => nameList.Contains(it.Name)).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ( ([p0].[Name] in @y0 ) or  ([p0].[Name] in @y1 )  ) or  ([p0].[Name] in @y2 )  )", r1MiddleResult.Sql);
            Assert.Equal(3, r1MiddleResult.SqlParameters.Count);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(p0, r1MiddleResult.SqlParameters[0].Value);

            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal(p1, r1MiddleResult.SqlParameters[1].Value);

            Assert.Equal("@y2", r1MiddleResult.SqlParameters[2].ParameterName);
            Assert.Equal(p2, r1MiddleResult.SqlParameters[2].Value);
        }

        /// <summary>
        /// colloection
        /// </summary>
        [Fact]
        public void TestWhere36()
        {
            var personRepository = new PersonRepository();
            var nameList = Enumerable.Range(0, 1001).Select(it => "hzp" + it).ToList();

            var p0 = nameList.Take(500).ToList();
            var p1 = nameList.Skip(500).Take(500).ToList();
            var p2 = nameList.Skip(1000).Take(500).ToList();

            var r1 = personRepository.Where(it => nameList.Contains(it.Name)).ToArray();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ( ([p0].[Name] in @y0 ) or  ([p0].[Name] in @y1 )  ) or  ([p0].[Name] in @y2 )  )", r1MiddleResult.Sql);
            Assert.Equal(3, r1MiddleResult.SqlParameters.Count);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(p0, r1MiddleResult.SqlParameters[0].Value);

            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal(p1, r1MiddleResult.SqlParameters[1].Value);

            Assert.Equal("@y2", r1MiddleResult.SqlParameters[2].ParameterName);
            Assert.Equal(p2, r1MiddleResult.SqlParameters[2].Value);
        }

        [Fact]
        public void TestWhere37()
        {
            var personRepository = new PersonRepository();
            var nameList = Enumerable.Range(0, 1001).Select(it => "hzp" + it).ToList();

            var p0 = nameList.Take(500).ToList();
            var p1 = nameList.Skip(500).Take(500).ToList();
            var p2 = nameList.Skip(1000).Take(500).ToList();

            var r1 = personRepository.Where(it => nameList.Contains(it.Name) && it.HaveChildren && it.Age == 15).ToArray();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ( ( ( ([p0].[Name] in @y0 ) or  ([p0].[Name] in @y1 )  ) or  ([p0].[Name] in @y2 )  ) AND  ([p0].[HaveChildren] = @y3 )  ) AND  ([p0].[Age] = @y4 )  )", r1MiddleResult.Sql);
            Assert.Equal(5, r1MiddleResult.SqlParameters.Count);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(p0, r1MiddleResult.SqlParameters[0].Value);

            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal(p1, r1MiddleResult.SqlParameters[1].Value);

            Assert.Equal("@y2", r1MiddleResult.SqlParameters[2].ParameterName);
            Assert.Equal(p2, r1MiddleResult.SqlParameters[2].Value);

            Assert.Equal("@y3", r1MiddleResult.SqlParameters[3].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[3].Value);
            Assert.Equal("@y4", r1MiddleResult.SqlParameters[4].ParameterName);
            Assert.Equal(15, r1MiddleResult.SqlParameters[4].Value);
        }


        [Fact]
        public void TestCombineSelectAndWhere()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name == "hzp").Select(it => it).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ([p0].[Name] = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestCombineSelectAndWhere2()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name == "hzp").Select(it => new { it.Age, Address = "福建" }).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Age], '福建' As [Address] FROM [Person] As [p0] WHERE  ([p0].[Name] = @y0 )", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestCombineSelectAndWhere3()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name == "hzp" && it.HaveChildren).Select(it => new { it.Age, it.HaveChildren }).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] WHERE  ( ([p0].[Name] = @y0 ) AND  ([p0].[HaveChildren] = @y1 )  )", r1MiddleResult.Sql);
            Assert.Equal(2, r1MiddleResult.SqlParameters.Count);

            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);

            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[1].Value);
        }

        [Fact]
        public void TestOrderBy()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.OrderBy(it=>it.Age).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] ORDER BY [p0].[Age]", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);
        }

        [Fact]
        public void TestOrderBy2()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.OrderByDescending(it => it.Age).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] ORDER BY [p0].[Age] DESC", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);
        }
        [Fact]
        public void TestOrderBy3()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.OrderBy(it => it.Age).ThenBy(it => it.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] ORDER BY [p0].[Age],[p0].[Name]", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);
        }

        [Fact]
        public void TestOrderBy4()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.OrderBy(it => it.Age).ThenByDescending(it => it.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] ORDER BY [p0].[Age],[p0].[Name] DESC", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);
        }

        [Fact]
        public void TestOrderBy5()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.OrderByDescending(it => it.Age).ThenByDescending(it => it.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] ORDER BY [p0].[Age] DESC,[p0].[Name] DESC", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);
        }

        [Fact]
        public void TestOrderBy6()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.OrderByDescending(it => it.Age).ThenBy(it => it.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] ORDER BY [p0].[Age] DESC,[p0].[Name]", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);
        }

        
        [Fact]
        public void TestGroupBy()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.GroupBy(it=>it.Name).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] GROUP BY [p0].[Name]", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);
        }

        [Fact]
        public void TestGroupBy2()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.GroupBy(it => new {it.Name,it.Age}).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] GROUP BY [p0].[Name],[p0].[Age]", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);
        }
        [Fact]
        public void TestGroupBy3()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.GroupBy(it => it.Name).Select(g => new { Key = g.Key, Count = g.Count(), Uv = g.Sum(it => it.Age) }).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();

            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0] GROUP BY [p0].[Name]", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);
        }
        [Fact]
        public void TestEf()
        {
            var c = new myDbContext();
            var d = c.person.GroupBy(it => new { it.Name, it.Age }).Select(g => new { Key = g.Key, Count = g.Count(), Uv = g.Sum(it => it.Age) }).ToList();
            //var d = c.person.OrderBy(it => new { it.Age, it.Name }).ToList();
            //.ToDictionary(g => g.Key, g => g.Count);
        }
    }

    public class myDbContext : DbContext
    {
        [Obsolete]
        public static readonly LoggerFactory LoggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() });
        public DbSet<Person> person { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory);
            optionsBuilder.UseMySQL("server=localhost;userid=root;password=123456;database=world;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
