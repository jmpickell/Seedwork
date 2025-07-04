using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Client;
using Moq;
using Seedwork.Repositories;
using Seedwork.Repositories.Interfaces;
using Seedwork.Repositories.SQL;
using Seedwork.Utilities.Specification.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Seedwork.Tests.Repositories.SQL
{
    [TestClass]
    public class SqlRepositoryTests
    {
        private Mock<IDbCommand> _mockCommand;
        private Mock<IDbConnection> _mockConnection;
        private Lazy<SqlRepository> _sut;

        [TestInitialize]
        public void Initialize()
        {
            _mockCommand = new Mock<IDbCommand>();
            _mockCommand.Setup(x => x.Parameters).Returns(new Mock<IDataParameterCollection>().Object);
            _mockCommand.Setup(x => x.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
            _mockConnection = new Mock<IDbConnection>();
            _mockConnection.Setup(x => x.CreateCommand()).Returns(() => _mockCommand.Object);
            _sut = new Lazy<SqlRepository>(() => new SqlRepository(() => _mockConnection.Object, NullLogger<SqlRepository>.Instance));
        }

        #region Create
        [TestMethod]
        [DataRow("ns", "table", "column", "")]
        [DataRow("ns", "table", ";1=1", "field")]
        [DataRow(null, "12_34~", "column", "field")]
        [DataRow("ns", "", "column", "field")]
        public void CreatePK_InvalidParameters_false(string ns, string table, string column, string field)
        {
            var query = new Query<object> { Namespace = ns, Column = column, Table = table, Key = new object() };
            var row = new Row(new Dictionary<string, object>
            {
                { field, new object() }
            });
            Assert.IsFalse(_sut.Value.Create(query, row));
        }

        [TestMethod]
        public void CreatePK_NoFields_false()
        {
            var query = new Query<object> { Namespace = "ns", Column = "column", Table = "table", Key = new object() };
            var row = new Row(new Dictionary<string, object>());
            Assert.IsFalse(_sut.Value.Create(query, row));
        }


        [TestMethod]
        [DataRow(0, false)]
        [DataRow(1, true)]
        public void CreatePK_NoIssues_TrueIfGT0(int recordsCreated, bool result)
        {
            _mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(() => recordsCreated);

            var query = new Query<object> { Namespace = "ns", Column = "column", Table = "table", Key = new object() };
            var row = new Row(new Dictionary<string, object>
            {
                { "field", new object() }
            });
            Assert.AreEqual(result, _sut.Value.Create(query, row));
        }
        #endregion

        #region Read
        [TestMethod]
        [DataRow("ns", "table", "column", "")]
        [DataRow("ns", "table", ";1=1", "field")]
        [DataRow(null, "12_34~", "column", "field")]
        [DataRow("ns", "", "column", "field")]
        public void ReadPK_InvalidParameters_NoFieldsReturned(string ns, string table, string column, string field)
        {
            var query = new Query<object> { Namespace = ns, Column = column, Table = table, Key = new object() };

            Assert.AreEqual(0, _sut.Value.Read(query, field).GetAll().Count);
        }

        public class MockWrapper<T> : Mock<T>, IMock
            where T : class
        {
            IMock IMock.SetupAllProperties()
            {
                var result = base.SetupAllProperties();
                return this;
            }
        }

        public interface IMock 
        { 
            object Object { get; }
            IMock SetupAllProperties();
        }

        [TestMethod]
        public void ReadPK_NoIssues_RowReturned()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.SetupSequence(x => x.Read()).Returns(true).Returns(true).Returns(false);
            mockReader.Setup(x => x.GetName(It.IsAny<int>())).Returns("field");
            mockReader.Setup(x => x.GetValue(It.IsAny<int>())).Returns(new object());

            _mockCommand.Setup(x => x.ExecuteReader()).Returns(mockReader.Object);

            var query = new Query<object> { Namespace = "ns", Column = "column", Table = "table", Key = new object() };

            Assert.AreEqual(1, _sut.Value.Read(query, "field").GetAll().Count);
        }

        [TestMethod]
        [DataRow("ns", "table", ";1=1")]
        [DataRow(null, "12_34~", "column")]
        [DataRow("ns", "", "column")]
        public void ReadFilter_InvalidParameters_NoFieldsReturned(string ns, string table, string column)
        {
            var query = new Query<ISpecification<Row>> { Namespace = ns, Column = column, Table = table, Key = new Mock<ISpecification<Row>>().Object };
            var result = _sut.Value.Read(query, "field");
            Assert.IsFalse(result.Any());
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadFilter_InvalidSpecification_Exception()
        {
            var query = new Query<ISpecification<Row>> { Namespace = "ns", Column = "column", Table = "table", Key = new Mock<ISpecification<Row>>().Object };
            var result = _sut.Value.Read(query, "field").ToArray();
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadFilter_InvalidCompositeSpecification_Exception()
        {
            var mockSpecification = new Mock<ICompositeSpecification<Row>>();
            mockSpecification.Setup(x => x.Left).Returns(Filter.Check("F1").LessThan(0));
            mockSpecification.Setup(x => x.Right).Returns(Filter.Check("F1").LessThan(0));

            var query = new Query<ISpecification<Row>> { Namespace = "ns", Column = "column", Table = "table", Key = mockSpecification.Object };
            var result = _sut.Value.Read(query, "field").ToArray();
            Assert.Fail();
        }

        [TestMethod]
        public void ReadFilter_NoIssue_RowsReturned()
        {
            var key =
                Filter.Check("f1").GreaterThan(70) &
                (Filter.Check("f2").GreaterThan(10000) & !Filter.Check("f4").LessThan(30000)) |
                Filter.Check("f3").Equals("Europe");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.SetupSequence(x => x.Read()).Returns(true).Returns(true).Returns(false);
            mockReader.Setup(x => x.GetName(It.IsAny<int>())).Returns("field");
            mockReader.Setup(x => x.GetValue(It.IsAny<int>())).Returns(new object());

            _mockCommand.Setup(x => x.ExecuteReader()).Returns(mockReader.Object);

            var query = new Query<ISpecification<Row>> { Namespace = "ns", Column = "column", Table = "table", Key = key };
            var result = _sut.Value.Read(query, "field");
            foreach (var row in result)
                Assert.AreEqual(1, row.GetAll().Count);
        }
        #endregion

        #region Update
        [TestMethod]
        [DataRow("ns", "table", "column", "")]
        [DataRow("ns", "table", ";1=1", "field")]
        [DataRow(null, "12_34~", "column", "field")]
        [DataRow("ns", "", "column", "field")]
        public void UpdatePK_InvalidParameters_false(string ns, string table, string column, string field)
        {
            var query = new Query<object> { Namespace = ns, Column = column, Table = table, Key = new object() };
            var row = new Row(new Dictionary<string, object>
            {
                { field, new object() }
            });
            Assert.IsFalse(_sut.Value.Update(query, row));
        }

        [TestMethod]
        public void UpdatePK_NoFields_false()
        {
            var query = new Query<object> { Namespace = "ns", Column = "column", Table = "table", Key = new object() };
            var row = new Row(new Dictionary<string, object>());
            Assert.IsFalse(_sut.Value.Update(query, row));
        }


        [TestMethod]
        [DataRow(0, false)]
        [DataRow(1, true)]
        public void UpdatePK_NoIssues_TrueIfGT0(int recordsCreated, bool result)
        {
            _mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(() => recordsCreated);

            var query = new Query<object> { Namespace = "ns", Column = "column", Table = "table", Key = new object() };
            var row = new Row(new Dictionary<string, object>
            {
                { "field", new object() }
            });
            Assert.AreEqual(result, _sut.Value.Update(query, row));
        }
        #endregion

        #region Delete
        [TestMethod]
        [DataRow("ns", "table", ";1=1")]
        [DataRow(null, "12_34~", "column")]
        [DataRow("ns", "", "column")]
        public void DeletePK_InvalidParameters_NoFieldsReturned(string ns, string table, string column)
        {
            var query = new Query<object> { Namespace = ns, Column = column, Table = table, Key = new object() };

            Assert.IsFalse(_sut.Value.Delete(query));
        }

        [TestMethod]
        [DataRow(0, false)]
        [DataRow(1, true)]
        public void DeletePK_NoIssues_TrueIfGT0(int recordsCreated, bool result)
        {
            _mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(() => recordsCreated);

            var query = new Query<object> { Namespace = "ns", Column = "column", Table = "table", Key = new object() };
            Assert.AreEqual(result, _sut.Value.Delete(query));
        }
        #endregion
    }
}
