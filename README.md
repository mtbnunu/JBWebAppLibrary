# jbwebapplibary

Set Up:

1. import this project as reference to your MVC5 web applicaiton project
2. Define connectionstring as you would in your web.config
3. Define Context as you would, including all models that should be put in database


CONTEXT EXAMPLE

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
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
    
4. Declare and initialize UnitOfWork before using it
    We recommend having a BaseController that holds all common variables including unitOfWork and having your Controllers inherit from this BaseController.

EXAMPLE
    
Declare as global variable:
    protected IUnitOfWork unitOfWork;
    
Initialize this variable in constructor:
    public BaseRepository(){
        unitOfWork = new UnitOfWork(SomeContext)
    }
    

Usage:
    Basic Usage:
    you can access the database for a model through following format, where method is a method provided by this library.

    unitOfWork.Repository<SomeModel>().method();
    
    Whenever you make change to a database(for example by insert() or update()), you need to call following to apply this changes:
    
    unitOfWork.Save();
    
List of Methods:
