using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen
{
	public class CodeParser
	{
		private readonly ContentTypeConfiguration configuration;
		private readonly IEnumerable<DataTypeDefinition> dataTypes;
	    private readonly ParserFactory parserFactory;
	    private readonly CSharpParser parser = new CSharpParser();

		public CodeParser(
            ContentTypeConfiguration configuration, 
            IEnumerable<DataTypeDefinition> dataTypes,
            ParserFactory parserFactory
        )
		{
			this.configuration = configuration;
			this.dataTypes = dataTypes;
		    this.parserFactory = parserFactory;
		}

		public IEnumerable<ContentType> Parse(TextReader reader)
		{
			var tree = parser.Parse(reader);
			ValidateTree(tree);
			return FindTypes(tree).Select(Generate);
		}

		private static void ValidateTree(SyntaxTree tree)
		{
			if (tree.Errors.Count > 0)
				throw new AnalysisException(tree);
		}

		private static IEnumerable<TypeDeclaration> FindTypes(AstNode tree)
		{
			return tree.Descendants.OfType<TypeDeclaration>();
		}

		private ContentType Generate(TypeDeclaration type)
		{
		    var typedParser = parserFactory.Create(configuration, dataTypes);
		    var contentType = typedParser.Parse(type);
		    return contentType;
		}
	}

    [Serializable]
	public class AnalysisException : Exception
	{
		public IEnumerable<string> Errors { get; private set; }

		public AnalysisException(SyntaxTree tree)
			: base("Errors in code analysis. See the Errors collection for details.")
		{
			Errors = tree.Errors.Select(e => e.Region.BeginLine + ": " + e.Message);
		}

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("errors", String.Join(";", Errors ?? new string[0]));
        }

        protected AnalysisException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Errors = ((string)info.GetValue("errors", typeof (string)) ?? "").Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries);
        }
	}
}