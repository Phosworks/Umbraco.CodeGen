﻿using System.Collections.Generic;
using Umbraco.CodeGen.Parsers.Annotated;

namespace Umbraco.CodeGen.Parsers
{
    public class AnnotatedParserFactory : ParserFactory
    {
        public override ContentTypeCodeParser CreateMediaTypeParser()
        {
            var parsers = CreateParsers(
                new CommonInfoParser(Configuration),
                new MediaPropertyParser(
                    new PropertyParser(Configuration, DataTypes), 
                    Configuration, 
                    DataTypes
                    )
                );
            return new MediaTypeCodeParser(Configuration, parsers.ToArray());
        }

        public override ContentTypeCodeParser CreateDocumentTypeParser()
        {
            var parsers = CreateParsers(
                new DocumentTypeInfoParser(Configuration), 
                new PropertyParser(Configuration, DataTypes)
                );
            return new DocumentTypeCodeParser(Configuration, parsers.ToArray());
        }

        protected List<ContentTypeCodeParserBase> CreateParsers(InfoParserBase infoParser, ContentTypeCodeParserBase propertyParser)
        {
            var parsers = CreateDocumentParsers(propertyParser);
            parsers.Insert(0, infoParser);
            return parsers;
        }

        protected List<ContentTypeCodeParserBase> CreateDocumentParsers(ContentTypeCodeParserBase propertyParser)
        {
            return new List<ContentTypeCodeParserBase>
            {
                new StructureParser(Configuration),
                new PropertiesParser(Configuration,
                    propertyParser
                    ),
                new TabsParser(Configuration)
            };
        }
    }
}
