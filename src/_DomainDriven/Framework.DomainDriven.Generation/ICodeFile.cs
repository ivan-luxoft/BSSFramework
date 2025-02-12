﻿using System;
using System.CodeDom;

using Framework.CodeDom;

using JetBrains.Annotations;

namespace Framework.DomainDriven.Generation
{
    public interface ICodeFile : IRenderingFile<CodeNamespace>
    {

    }

    public static class CodeFileExtensions
    {
        public static ICodeFile WithVisitor([NotNull] this ICodeFile codeFile, [NotNull] CodeDomVisitor visitor)
        {
            if (codeFile == null) throw new ArgumentNullException(nameof(codeFile));
            if (visitor == null) throw new ArgumentNullException(nameof(visitor));

            return new VisitedCodeFile(codeFile, visitor);
        }


        private class VisitedCodeFile : ICodeFile
        {
            private readonly ICodeFile _baseCodeFile;

            private readonly CodeDomVisitor _visitor;


            public VisitedCodeFile([NotNull] ICodeFile baseCodeFile, [NotNull] CodeDomVisitor visitor)
            {
                if (baseCodeFile == null) throw new ArgumentNullException(nameof(baseCodeFile));
                if (visitor == null) throw new ArgumentNullException(nameof(visitor));

                this._baseCodeFile = baseCodeFile;
                this._visitor = visitor;
            }


            public string Filename
            {
                get { return this._baseCodeFile.Filename; }
            }


            public CodeNamespace GetRenderData()
            {
                return this._visitor.VisitNamespace(this._baseCodeFile.GetRenderData());
            }
        }
    }
}
