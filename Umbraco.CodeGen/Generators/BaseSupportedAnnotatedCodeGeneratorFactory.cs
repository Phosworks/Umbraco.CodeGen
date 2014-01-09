﻿using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators.Annotated;
using Umbraco.CodeGen.Generators.BaseSupportedAnnotated;
using ImportsGenerator = Umbraco.CodeGen.Generators.BaseSupportedAnnotated.ImportsGenerator;

namespace Umbraco.CodeGen.Generators
{
    public class BaseSupportedAnnotatedCodeGeneratorFactory : CodeGeneratorFactory
    {
        public override CodeGeneratorBase Create(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            if (configuration.ContentTypeName == "DocumentType")
                return CreateDocTypeGenerator(configuration, dataTypes);
            return CreateMediaTypeGenerator(configuration, dataTypes);
        }

        public CodeGeneratorBase CreateDocTypeGenerator(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return CreateGenerators(
                configuration,
                dataTypes,
                "DocumentType",
                new DocumentTypeInfoGenerator(configuration)
                );
        }

        public CodeGeneratorBase CreateMediaTypeGenerator(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return CreateGenerators(
                configuration,
                dataTypes,
                "MediaType",
                new CommonInfoGenerator(configuration)
                );
        }

        private static CodeGeneratorBase CreateGenerators(
            ContentTypeConfiguration configuration,
            IEnumerable<DataTypeDefinition> dataTypes,
            string attributeName,
            CodeGeneratorBase infoGenerator)
        {
            return new NamespaceGenerator(
                configuration,
                new ImportsGenerator(configuration),
                new ClassGenerator(configuration,
                    new CompositeCodeGenerator(
                        configuration,
                        new EntityNameGenerator(configuration),
                        new AttributeCodeGenerator(
                            attributeName,
                            configuration,
                            infoGenerator,
                            new StructureGenerator(configuration)
                            )
                        ),
                    new BaseSupportedAnnotated.CtorGenerator(configuration),
                    new PropertiesGenerator(
                        configuration,
                        new PropertyDeclarationGenerator(
                            configuration,
                            dataTypes.ToList(),
                            new EntityNameGenerator(configuration),
                            new AttributeCodeGenerator(
                                "GenericProperty",
                                configuration,
                                new PropertyInfoGenerator(configuration, dataTypes.ToList())
                                ),
                            new BaseSupportedAnnotated.PropertyBodyGenerator(configuration)
                            )
                        )
                    )
                );
        }
    }
}
