namespace CalcsGenerator.DataModel
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ProjectsContext : DbContext
    {
        public ProjectsContext() : base("name=ProjectContext")
        {

            
        }

        public DbSet<Project> Projects { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}