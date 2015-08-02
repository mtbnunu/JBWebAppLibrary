# jbwebapplibary

CONTEXT EXAMPLE

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DefaultContext : DbContext
    {
        public DefaultContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Participant> participants { get; set; }
        public DbSet<SurveryQuestion> SurveryQuestions { get; set; }
        public DbSet<SurveyAnswer> SurveyAnswers { get; set; }
    }
    
    
  
