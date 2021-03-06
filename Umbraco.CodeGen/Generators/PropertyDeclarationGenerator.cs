﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class PropertyDeclarationGenerator : CodeGeneratorBase
    {
        protected IList<DataTypeDefinition> DataTypes;
        protected CodeGeneratorBase[] MemberGenerators;

        public PropertyDeclarationGenerator(
            ContentTypeConfiguration config,
            IList<DataTypeDefinition> dataTypes,
            params CodeGeneratorBase[] memberGenerators
            ) : base(config)
        {
            this.DataTypes = dataTypes;
            this.MemberGenerators = memberGenerators;
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var property = (GenericProperty)entity;
            var propNode = (CodeMemberProperty)codeObject;

            SetType(propNode, property);

            foreach (var generator in MemberGenerators)
                generator.Generate(codeObject, property);

            SetPublic(propNode);
        }

        protected void SetPublic(CodeTypeMember propNode)
        {
            propNode.Attributes = MemberAttributes.Public;
        }

        protected void SetType(CodeMemberProperty propNode, GenericProperty property)
        {
            var hasType = property.Type != null &&
                          Config.TypeMappings.ContainsKey(property.Type.ToLower());
            var typeName = hasType 
                ? Config.TypeMappings[property.Type.ToLower()]
                : Config.TypeMappings.DefaultType;
            if (typeName == null)
                throw new Exception("TypeMappings/Default not set. Cannot guess default property type.");
            propNode.Type = new CodeTypeReference(typeName);
        }
    }
}