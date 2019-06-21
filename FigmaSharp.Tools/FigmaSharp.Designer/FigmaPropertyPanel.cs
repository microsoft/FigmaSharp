///* 
// * FigmaViewContent.cs 
// * 
// * Author:
// *   Jose Medrano <josmed@microsoft.com>
// *
// * Copyright (C) 2018 Microsoft, Corp
// *
// * Permission is hereby granted, free of charge, to any person obtaining
// * a copy of this software and associated documentation files (the
// * "Software"), to deal in the Software without restriction, including
// * without limitation the rights to use, copy, modify, merge, publish,
// * distribute, sublicense, and/or sell copies of the Software, and to permit
// * persons to whom the Software is furnished to do so, subject to the
// * following conditions:
// * 
// * The above copyright notice and this permission notice shall be included in
// * all copies or substantial portions of the Software.
// *
// * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// * USE OR OTHER DEALINGS IN THE SOFTWARE.
// */
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using AppKit;
////using Xamarin.PropertyEditing;
////using Xamarin.PropertyEditing.Mac;
////using Xamarin.PropertyEditing.Reflection;

//namespace MonoDevelop.Figma
//{
//    public class FigmaPropertyPanel
//    {
//        public NSView View => propertyEditorPanel;

//        //PropertyEditorPanel propertyEditorPanel;
//        public FigmaPropertyPanel()
//        {
//            propertyEditorPanel = new PropertyEditorPanel();
//        }

//        public void Select(object obj)
//        {
//            propertyEditorPanel.Select(new[] { obj });
//        }

//        public void Initialize ()
//        {
//            var provider = new FigmaEditorProvider();
//            propertyEditorPanel.TargetPlatform = new TargetPlatform(provider)
//            {
//                //AutoExpandGroups = new[] { "ReadWrite" }
//            };
//        }
//    }

//    public class FigmaEditorProvider : IEditorProvider
//    {
//        public IReadOnlyDictionary<Type, ITypeInfo> KnownTypes { get; } = new Dictionary<Type, ITypeInfo> { };
//        private readonly Dictionary<object, IObjectEditor> editorCache = new Dictionary<object, IObjectEditor>();
//        public Task<object> CreateObjectAsync(ITypeInfo type)
//        {
//            Type realType = Type.GetType($"{type.NameSpace}.{type.Name}, {type.Assembly.Name}");
//            if (realType == null)
//                return Task.FromResult<object>(null);

//            return Task.FromResult(Activator.CreateInstance(realType));
//        }

//        public Task<AssignableTypesResult> GetAssignableTypesAsync(ITypeInfo type, bool childTypes)
//            => Task.FromResult(new AssignableTypesResult(new List<ITypeInfo>()));

//        public Task<IReadOnlyList<object>> GetChildrenAsync(object item)
//            => Task.FromResult((IReadOnlyList<object>)Array.Empty<object>());

//        public Task<IObjectEditor> GetObjectEditorAsync(object item)
//        {
//            if (this.editorCache.TryGetValue(item, out IObjectEditor cachedEditor))
//            {
//                return Task.FromResult(cachedEditor);
//            }
//            IObjectEditor editor = new ReflectionObjectEditor(item);
//            this.editorCache.Add(item, editor);
//            return Task.FromResult(editor);
//        }

//        public async Task<IReadOnlyCollection<IPropertyInfo>> GetPropertiesForTypeAsync(ITypeInfo type)
//        {
//            Type realType = ReflectionEditorProvider.GetRealType(type);
//            if (realType == null)
//                return Array.Empty<IPropertyInfo>();
//            return ReflectionEditorProvider.GetPropertiesForType(realType);
//        }
//    }
//}