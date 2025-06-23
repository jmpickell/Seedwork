using Microsoft.Identity.Client.Extensions.Msal;
using Moq;
using Seedwork.Repositories;
using Seedwork.Repositories.SQL;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Seedwork.Tests.Repositories.SQL
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void AddParameter_NoIssues_ParameterAdded()
        {
            bool hit = false;
            var mockCollection = new Mock<IDataParameterCollection>();
            mockCollection.Setup(x => x.Add(It.IsAny<object>())).Callback((object o) => hit = true).Returns(1);

            var mockParameter = new Mock<IDbDataParameter>();
            var sut = new Mock<IDbCommand>();
            sut.Setup(x => x.CreateParameter()).Returns(mockParameter.Object);
            sut.Setup(x => x.Parameters).Returns(mockCollection.Object);

            sut.Object.AddParameter("name", "value");

            Assert.IsTrue(hit);
        }

        [TestMethod]
        public void ToDictionary_NoIssues_DictionaryReturned()
        {
            var sut = new Mock<IDataReader>();
            sut.Setup(x => x.FieldCount).Returns(1);
            sut.Setup(x => x.GetName(It.IsAny<int>())).Returns("name");
            sut.Setup(x => x.GetValue(It.IsAny<int>())).Returns("value");

            Assert.AreEqual(1, sut.Object.ToDictionary().Count);
        }

        [TestMethod]
        public void AsSql_NoIssues_SqlValidStringReturned()
        {
            Assert.AreEqual("a,b,c", new[] { "a", "b", "c" }.AsSql());
            Assert.AreEqual("*", new string[0].AsSql());
        }

        [TestMethod]
        [DataRow("ns", "table", ";1=1")]
        [DataRow(null, "12_34~", "column")]
        [DataRow("~~~~", "table", "column")]
        public void IsValidSqlFieldQ_InvalidParameters_False(string ns, string table, string column)
        {
            var sut = new Query<object> { Namespace = ns, Column = column, Table = table, Key = new object() };
            Assert.IsFalse(sut.IsValidSqlField());
        }

        [TestMethod]
        public void IsValidSqlFieldQ_NoIssues_True()
        {
            var sut = new Query<object> { Namespace = "ns", Column = "column", Table = "table", Key = new object() };
            Assert.IsTrue(sut.IsValidSqlField());
        }

        [TestMethod]
        public void IsValidSqlFieldE_NoExceptions_Validated()
        {
            Assert.IsFalse(new[] { "good", "bad!" }.IsValidSqlField());
            Assert.IsTrue(new[] { "good", "valid" }.IsValidSqlField());
        }

        [TestMethod]
        public void IsValidSqlField_NoExceptions_Validated()
        {
            Assert.IsFalse("".IsValidSqlField());
            Assert.IsFalse("~_!nv41!d_~".IsValidSqlField());
            Assert.IsTrue("_sut_123".IsValidSqlField());
        }

        [TestMethod]
        [DataRow("namespace.table","namespace","table")]
        [DataRow("table", "", "table")]
        [DataRow("table", null, "table")]
        public void BuildTableName_NoExceptions_TableNameReturned(string result, string ns, string table)
        {
            var sut = new Query<object> { Namespace = ns, Table = table };
            Assert.AreEqual(result, sut.BuildTableName());
        }

        [TestMethod]
        public void GetColumnNames()
        {
            Assert.IsNotNull(new Row(new Dictionary<string, object>()).GetColumnNames());
        }

        [TestMethod]
        public void IsNullOrEmpty_Executed_NoIssues()
        {
            Assert.IsFalse("sut".IsNullOrEmpty());
            Assert.IsTrue("".IsNullOrEmpty());
            Assert.IsTrue(((string)null).IsNullOrEmpty());
        }
    }
}
