**Set Up:**
-----------

- Import this project as reference to your MVC5 web applicaiton project

- Define ***[connectionstring](https://www.connectionstrings.com/sqlconnection/)*** as you would in your web.config

- Define ***Context*** as you would, including all models that should be put in database

EXAMPLE:

    public class SomeContext : DbContext
    {
        public SomeContext()
            : base("name_of_your_connectionstring")
        {
        }
        public DbSet<SomeModel> someModels { get; set; }
        public DbSet<SomeOtherModel> someOtherModels { get; set; }
        ...
    }

- Declare and initialize UnitOfWork before using it.

 *We recommend having a **BaseController** that holds all common variables including unitOfWork and having your Controllers inherit from this BaseController.*

**EXAMPLE:**

Declare as global variable: 
	
	protected IUnitOfWork unitOfWork;

Initialize this variable in constructor: 
	
	public BaseRepository(){ 
	            unitOfWork = new UnitOfWork(new SomeContext());
		}

- You can use **Package Manager Console** and original **[Migration](http://www.asp.net/mvc/overview/getting-started/getting-started-with-ef-using-mvc/migrations-and-deployment-with-the-entity-framework-in-an-asp-net-mvc-application)** methods to deploy/update database

**Usage:**
----------

**Basic Usage:**
You can access the database for a model through following format, where method is a method provided by this library.

	unitOfWork.Repository<SomeModel>().method();

Whenever you make **any change** to a database (for example by insert() or update()), you need to call following to apply this changes:

	unitOfWork.Save();

**List of Methods:**
--------------------

