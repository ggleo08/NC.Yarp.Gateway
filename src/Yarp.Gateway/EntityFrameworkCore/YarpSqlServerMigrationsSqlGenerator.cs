using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yarp.Gateway.EntityFrameworkCore
{
    public class YarpSqlServerMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        public YarpSqlServerMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies,
                                                   IRelationalAnnotationProvider annotationProvider)
            : base(dependencies, annotationProvider)
        {

        }

        protected override void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            operation.ForeignKeys.Clear();
            base.Generate(operation, model, builder, terminate);
        }

        //protected override void Generate(AddForeignKeyOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true) { }
        //protected override void Generate(DropForeignKeyOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate) { }
    }

}
