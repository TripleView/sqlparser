using DatabaseParser.Base;
using DatabaseParser.ExpressionParser;
using System.Linq;
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
    }
    public class PersonRepository : BaseRepository<Person>, IPersonRepository
    {
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
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM (SELECT [Name], [Age], [HaveChildren] FROM [Person]) As p0", r1MiddleResult.Sql);
            Assert.Empty(r1MiddleResult.SqlParameters);

            personRepository = new PersonRepository();
            var r2 = personRepository.Select(it => it.Name).ToList();
            var r2MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name] FROM (SELECT [Name], [Age], [HaveChildren] FROM [Person]) As p0", r2MiddleResult.Sql);
            Assert.Empty(r2MiddleResult.SqlParameters);

            personRepository = new PersonRepository();
            var r3 = personRepository.Select(it => new { it.Name, Address = "福建" }).ToList();
            var r3MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], '福建' As [Address] FROM (SELECT [Name], [Age], [HaveChildren] FROM [Person]) As p0", r3MiddleResult.Sql);
            Assert.Empty(r3MiddleResult.SqlParameters);

            personRepository = new PersonRepository();
            var pet = new Pet() { Name = "Dog" };
            var r4 = personRepository.Select(it => new { it.Name, Address = pet.Name }).ToList();
            var r4MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], 'Dog' As [Address] FROM (SELECT [Name], [Age], [HaveChildren] FROM [Person]) As p0", r4MiddleResult.Sql);
            Assert.Empty(r4MiddleResult.SqlParameters);

        }

        [Fact]
        public void TestWhere()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.HaveChildren).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE [p0].[HaveChildren] = @y0", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[0].Value);


        }

        [Fact]
        public void TestWhere2()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.HaveChildren && it.Name == "hzp" && it.Age == 15).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE [p0].[HaveChildren] = @y0 AND [p0].[Name] = @y1 AND [p0].[Age] = @y2", r1MiddleResult.Sql);
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
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE [p0].[HaveChildren] = @y0 AND [p0].[Name] = @y1 AND [p0].[Age] = @y2", r1MiddleResult.Sql);
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
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE [p0].[Name] = @y0 AND [p0].[HaveChildren] = @y1 AND [p0].[Age] = @y2", r1MiddleResult.Sql);
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
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE [p0].[Name] = @y0 AND [p0].[HaveChildren] = @y1 AND [p0].[Age] = @y2", r1MiddleResult.Sql);
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
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE [p0].[HaveChildren] = @y0 AND [p0].[HaveChildren] = @y1", r1MiddleResult.Sql);
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
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE [p0].[HaveChildren] = @y0", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(0, r1MiddleResult.SqlParameters[0].Value);
        }
        [Fact]
        public void TestWhere8()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => !!it.HaveChildren).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE [p0].[HaveChildren] = @y0", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal(0, r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere9()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name.Length > 3).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE LEN([p0].[Name]) > @y0", r1MiddleResult.Sql);
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
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE [p0].[Name] = @y0", r1MiddleResult.Sql);
            Assert.Single(r1MiddleResult.SqlParameters);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("小黄", r1MiddleResult.SqlParameters[0].Value);
        }

        [Fact]
        public void TestWhere11()
        {
            var personRepository = new PersonRepository();
            var r1 = personRepository.Where(it => it.Name == "hzp" &&( it.HaveChildren && it.Age == 15)).ToList();
            var r1MiddleResult = personRepository.GetDbQueryDetail();
            Assert.Equal("SELECT [p0].[Name], [p0].[Age], [p0].[HaveChildren] FROM [Person] As [p0]  WHERE [p0].[Name] = @y0 AND [p0].[HaveChildren] = @y1 AND [p0].[Age] = @y2", r1MiddleResult.Sql);
            Assert.Equal(3, r1MiddleResult.SqlParameters.Count);
            Assert.Equal("@y1", r1MiddleResult.SqlParameters[1].ParameterName);
            Assert.Equal(1, r1MiddleResult.SqlParameters[1].Value);
            Assert.Equal("@y0", r1MiddleResult.SqlParameters[0].ParameterName);
            Assert.Equal("hzp", r1MiddleResult.SqlParameters[0].Value);
            Assert.Equal("@y2", r1MiddleResult.SqlParameters[2].ParameterName);
            Assert.Equal(15, r1MiddleResult.SqlParameters[2].Value);
        }
    }
}
