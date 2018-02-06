///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;

using com.espertech.esper.client;
using com.espertech.esper.client.scopetest;
using com.espertech.esper.client.soda;
using com.espertech.esper.compat;
using com.espertech.esper.metrics.instrumentation;
using com.espertech.esper.supportregression.bean.bookexample;
using com.espertech.esper.supportregression.bean.word;
using com.espertech.esper.supportregression.client;

using NUnit.Framework;

namespace com.espertech.esper.regression.epl
{
    [TestFixture]
	public class TestContainedEventSimple 
	{
	    private readonly string NEWLINE = Environment.NewLine;

	    private EPServiceProvider _epService;
	    private SupportUpdateListener _listener;

        [SetUp]
	    public void SetUp()
	    {
	        _epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.GetConfiguration());
	        _epService.Initialize();
	        if (InstrumentationHelper.ENABLED) { InstrumentationHelper.StartTest(_epService, this.GetType(), GetType().FullName);}
	        _listener = new SupportUpdateListener();
	    }

        [TearDown]
	    public void TearDown() {
	        if (InstrumentationHelper.ENABLED) { InstrumentationHelper.EndTest();}
	        _listener = null;
	    }

	    // Assures that the events inserted into the named window are preemptive to events generated by contained-event syntax.
	    // This example generates 3 contained-events: One for each book.
	    // It then inserts them into a named window to determine the highest price among all.
	    // The named window updates first becoming useful to subsequent events (versus last and not useful).
        [Test]
	    public void TestNamedWindowPremptive()
	    {
	        string[] fields = "bookId".Split(',');
	        _epService.EPAdministrator.Configuration.AddEventType("OrderEvent", typeof(OrderBean));
	        _epService.EPAdministrator.Configuration.AddEventType("BookDesc", typeof(BookDesc));

	        string stmtText = "insert into BookStream select * from OrderEvent[books]";
	        EPStatement stmt = _epService.EPAdministrator.CreateEPL(stmtText);
	        stmt.AddListener(_listener);

	        EPStatement stmtNW = _epService.EPAdministrator.CreateEPL("create window MyWindow#lastevent as BookDesc");
	        _epService.EPAdministrator.CreateEPL("insert into MyWindow select * from BookStream bs where not exists (select * from MyWindow mw where mw.price > bs.price)");

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventOne());
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, fields, new object[][] { new object[] { "10020" }, new object[] { "10021" }, new object[] { "10022" } });
	        _listener.Reset();

	        // higest price (27 is the last value)
            EventBean theEvent = stmtNW.First();
	        Assert.AreEqual(35.0, theEvent.Get("price"));
	    }

        [Test]
	    public void TestUnidirectionalJoin()
	    {
	        _epService.EPAdministrator.Configuration.AddEventType("OrderEvent", typeof(OrderBean));
	        string stmtText = "select * from " +
	                      "OrderEvent as orderEvent unidirectional, " +
	                      "OrderEvent[select * from books] as book, " +
	                      "OrderEvent[select * from orderdetail.items] as item " +
	                      "where book.bookId=item.productId " +
	                      "order by book.bookId, item.amount";
	        string stmtTextFormatted = "select *" + NEWLINE +
	                      "from OrderEvent as orderEvent unidirectional," + NEWLINE +
	                      "OrderEvent[select * from books] as book," + NEWLINE +
	                      "OrderEvent[select * from orderdetail.items] as item" + NEWLINE +
	                      "where book.bookId=item.productId" + NEWLINE +
	                      "order by book.bookId, item.amount";

	        EPStatement stmt = _epService.EPAdministrator.CreateEPL(stmtText);
	        stmt.AddListener(_listener);

	        RunAssertion();

	        stmt.Dispose();
	        EPStatementObjectModel model = _epService.EPAdministrator.CompileEPL(stmtText);
	        Assert.AreEqual(stmtText, model.ToEPL());
	        Assert.AreEqual(stmtTextFormatted, model.ToEPL(new EPStatementFormatter(true)));
	        stmt = _epService.EPAdministrator.Create(model);
	        stmt.AddListener(_listener);

	        RunAssertion();
	    }

	    private void RunAssertion()
	    {
	        string[] fields = "orderEvent.orderdetail.orderId,book.bookId,book.title,item.amount".Split(',');
	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventOne());
	        Assert.AreEqual(3, _listener.LastNewData.Length);
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, fields, new object[][] { new object[] { "PO200901", "10020", "Enders Game", 10 }, new object[] { "PO200901", "10020", "Enders Game", 30 }, new object[] { "PO200901", "10021", "Foundation 1", 25 } });
	        _listener.Reset();

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventTwo());
	        Assert.AreEqual(1, _listener.LastNewData.Length);
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, fields, new object[][] { new object[] { "PO200902", "10022", "Stranger in a Strange Land", 5 } });
	        _listener.Reset();

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventThree());
	        Assert.AreEqual(1, _listener.LastNewData.Length);
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, fields, new object[][] { new object[] { "PO200903", "10021", "Foundation 1", 50 } });
	    }

        [Test]
	    public void TestUnidirectionalJoinCount()
	    {
	        _epService.EPAdministrator.Configuration.AddEventType("OrderEvent", typeof(OrderBean));
	        string stmtText = "select count(*) from " +
	                      "OrderEvent orderEvent unidirectional, " +
	                      "OrderEvent[books] as book, " +
	                      "OrderEvent[orderdetail.items] item " +
	                      "where book.bookId = item.productId order by book.bookId asc, item.amount asc";

	        EPStatement stmt = _epService.EPAdministrator.CreateEPL(stmtText);
	        stmt.AddListener(_listener);

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventOne());
	        EPAssertionUtil.AssertProps(_listener.AssertOneGetNewAndReset(), "count(*)".Split(','), new object[]{3L});

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventTwo());
	        EPAssertionUtil.AssertProps(_listener.AssertOneGetNewAndReset(), "count(*)".Split(','), new object[]{1L});

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventThree());
	        EPAssertionUtil.AssertProps(_listener.AssertOneGetNewAndReset(), "count(*)".Split(','), new object[]{1L});

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventFour());
	        Assert.IsFalse(_listener.IsInvoked);
	    }

        [Test]
	    public void TestJoinCount()
	    {
	        string[] fields = "count(*)".Split(',');
	        _epService.EPAdministrator.Configuration.AddEventType("OrderEvent", typeof(OrderBean));
	        string stmtText = "select count(*) from " +
	                      "OrderEvent[books]#unique(bookId) book, " +
	                      "OrderEvent[orderdetail.items]#keepall item " +
	                      "where book.bookId = item.productId";

	        EPStatement stmt = _epService.EPAdministrator.CreateEPL(stmtText);
	        stmt.AddListener(_listener);

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventOne());
	        EPAssertionUtil.AssertProps(_listener.AssertOneGetNewAndReset(), fields, new object[]{3L});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new object[][] { new object[] { 3L } });

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventTwo());
	        EPAssertionUtil.AssertProps(_listener.AssertOneGetNewAndReset(), "count(*)".Split(','), new object[]{4L});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new object[][] { new object[] { 4L } });

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventThree());
	        EPAssertionUtil.AssertProps(_listener.AssertOneGetNewAndReset(), "count(*)".Split(','), new object[]{5L});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new object[][] { new object[] { 5L } });

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventFour());
	        Assert.IsFalse(_listener.IsInvoked);

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventOne());
	        EPAssertionUtil.AssertProps(_listener.AssertOneGetNewAndReset(), "count(*)".Split(','), new object[]{8L});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new object[][] { new object[] { 8L } });
	    }

        [Test]
	    public void TestJoin()
	    {
	        string[] fields = "book.bookId,item.itemId,amount".Split(',');
	        _epService.EPAdministrator.Configuration.AddEventType("OrderEvent", typeof(OrderBean));
	        string stmtText = "select book.bookId,item.itemId,amount from " +
	                      "OrderEvent[books]#firstunique(bookId) book, " +
	                      "OrderEvent[orderdetail.items]#keepall item " +
	                      "where book.bookId = item.productId " +
	                      "order by book.bookId, item.itemId";

	        EPStatement stmt = _epService.EPAdministrator.CreateEPL(stmtText);
	        stmt.AddListener(_listener);

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventOne());
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, fields, new object[][] { new object[] { "10020", "A001", 10 }, new object[] { "10020", "A003", 30 }, new object[] { "10021", "A002", 25 } });
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new object[][] { new object[] { "10020", "A001", 10 }, new object[] { "10020", "A003", 30 }, new object[] { "10021", "A002", 25 } });
	        _listener.Reset();

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventTwo());
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, fields, new object[][] { new object[] { "10022", "B001", 5 } });
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new object[][] { new object[] { "10020", "A001", 10 }, new object[] { "10020", "A003", 30 }, new object[] { "10021", "A002", 25 }, new object[] { "10022", "B001", 5 } });
	        _listener.Reset();

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventThree());
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, fields, new object[][] { new object[] { "10021", "C001", 50 } });
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new object[][] { new object[] { "10020", "A001", 10 }, new object[] { "10020", "A003", 30 }, new object[] { "10021", "A002", 25 }, new object[] { "10021", "C001", 50 }, new object[] { "10022", "B001", 5 } });
	        _listener.Reset();

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventFour());
	        Assert.IsFalse(_listener.IsInvoked);
	    }

        [Test]
	    public void TestAloneCount()
	    {
	        string[] fields = "count(*)".Split(',');
	        _epService.EPAdministrator.Configuration.AddEventType("OrderEvent", typeof(OrderBean));

	        string stmtText = "select count(*) from OrderEvent[books]";
	        EPStatement stmt = _epService.EPAdministrator.CreateEPL(stmtText);
	        stmt.AddListener(_listener);

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventOne());
	        EPAssertionUtil.AssertProps(_listener.AssertOneGetNewAndReset(), fields, new object[]{3L});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new object[][] { new object[] { 3L } });

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventFour());
	        EPAssertionUtil.AssertProps(_listener.AssertOneGetNewAndReset(), fields, new object[]{5L});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new object[][] { new object[] { 5L } });
	    }

        [Test]
	    public void TestPropertyAccess()
	    {
	        _epService.EPAdministrator.Configuration.AddEventType("OrderEvent", typeof(OrderBean));

	        EPStatement stmtOne = _epService.EPAdministrator.CreateEPL("@IterableUnbound select bookId from OrderEvent[books]");
	        stmtOne.AddListener(_listener);
	        EPStatement stmtTwo = _epService.EPAdministrator.CreateEPL("@IterableUnbound select books[0].author as val from OrderEvent(books[0].bookId = '10020')");

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventOne());
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, "bookId".Split(','), new object[][] { new[] { "10020" }, new[] { "10021" }, new[] { "10022" } });
	        _listener.Reset();
            EPAssertionUtil.AssertPropsPerRow(stmtOne.GetEnumerator(), "bookId".Split(','), new object[][] { new[] { "10020" }, new[] { "10021" }, new[] { "10022" } });
            EPAssertionUtil.AssertPropsPerRow(stmtTwo.GetEnumerator(), "val".Split(','), new object[][] { new[] { "Orson Scott Card" } });

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventFour());
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, "bookId".Split(','), new object[][] { new[] { "10031" }, new[] { "10032" } });
	        _listener.Reset();
            EPAssertionUtil.AssertPropsPerRow(stmtOne.GetEnumerator(), "bookId".Split(','), new object[][] { new[] { "10031" }, new[] { "10032" } });
            EPAssertionUtil.AssertPropsPerRow(stmtTwo.GetEnumerator(), "val".Split(','), new object[][] { new[] { "Orson Scott Card" } });

	        // add where clause
	        stmtOne.Dispose();
	        stmtTwo.Dispose();
	        stmtOne = _epService.EPAdministrator.CreateEPL("select bookId from OrderEvent[books where author='Orson Scott Card']");
	        stmtOne.AddListener(_listener);
	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventOne());
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, "bookId".Split(','), new object[][] { new[] { "10020" } });
	        _listener.Reset();
	    }

        [Test]
	    public void TestIRStreamArrayItem()
	    {
	        _epService.EPAdministrator.Configuration.AddEventType("OrderEvent", typeof(OrderBean));
	        string stmtText = "@IterableUnbound select irstream bookId from OrderEvent[books[0]]";

	        EPStatement stmt = _epService.EPAdministrator.CreateEPL(stmtText);
	        stmt.AddListener(_listener);

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventOne());
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, "bookId".Split(','), new object[][] { new[] { "10020" } });
	        Assert.IsNull(_listener.LastOldData);
	        _listener.Reset();
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), "bookId".Split(','), new object[][] { new[] { "10020" } });

	        _epService.EPRuntime.SendEvent(OrderBeanFactory.MakeEventFour());
	        Assert.IsNull(_listener.LastOldData);
            EPAssertionUtil.AssertPropsPerRow(_listener.LastNewData, "bookId".Split(','), new object[][] { new[] { "10031" } });
	        _listener.Reset();
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), "bookId".Split(','), new object[][] { new[] { "10031" } });
	    }

        [Test]
	    public void TestSplitWords() {
	        _epService.EPAdministrator.Configuration.AddEventType(typeof(SentenceEvent));
	        string stmtText = "insert into WordStream select * from SentenceEvent[words]";

	        string[] fields = "word".Split(',');
	        EPStatement stmt = _epService.EPAdministrator.CreateEPL(stmtText);
	        stmt.AddListener(_listener);

	        _epService.EPRuntime.SendEvent(new SentenceEvent("I am testing this"));
            EPAssertionUtil.AssertPropsPerRow(_listener.GetAndResetLastNewData(), fields, new object[][] { new[] { "I" }, new[] { "am" }, new[] { "testing" }, new[] { "this" } });
	    }

        [Test]
        public void TestArrayProperty()
        {
            _epService.EPAdministrator.Configuration.AddEventType<MyBeanWithArray>();
            _epService.EPAdministrator.CreateEPL("create objectarray schema ContainedId(id string)");
            EPStatement stmt = _epService.EPAdministrator.CreateEPL("select * from MyBeanWithArray[select topId, * from containedIds @type(ContainedId)]");
            stmt.AddListener(_listener);
            _epService.EPRuntime.SendEvent(new MyBeanWithArray("A", "one,two,three".SplitCsv()));
            EPAssertionUtil.AssertPropsPerRow(_listener.GetAndResetLastNewData(), "topId,id".SplitCsv(),
                    new Object[][] { new object[] { "A", "one" }, new object[] { "A", "two" }, new object[] { "A", "three" } });
        }

        private static BookDesc[] BookDesc
        {
            get
            {
                return new BookDesc[]
                {
                    new BookDesc(
                        "10020", "Enders Game", "Orson Scott Card", 24.00d,
                        new Review[]
                        {
                            new Review(1, "best book ever"),
                            new Review(2, "good science fiction")
                        }),
                    new BookDesc(
                        "10021", "Foundation 1", "Isaac Asimov", 35.00d,
                        new Review[]
                        {
                            new Review(10, "great book")
                        }),
                    new BookDesc("10022", "Stranger in a Strange Land", "Robert A Heinlein", 27.00d, new Review[0])
                };
            }
        }

        public class MyBeanWithArray
        {
            public MyBeanWithArray(String topId, String[] containedIds)
            {
                TopId = topId;
                ContainedIds = containedIds;
            }

            public string TopId { get; private set; }

            public string[] ContainedIds { get; private set; }
        }
	}
} // end of namespace
