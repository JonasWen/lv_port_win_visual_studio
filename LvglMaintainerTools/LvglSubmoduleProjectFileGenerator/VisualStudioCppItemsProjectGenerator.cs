﻿using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace LvglSubmoduleProjectFileGenerator
{
    public class VisualStudioCppItemsProjectGenerator
    {
        internal string defaultNamespace =
            @"http://schemas.microsoft.com/developer/msbuild/2003";

        internal XmlDocument projectDocument = null;
        internal XmlDocument filtersDocument = null;

        List<string> FilterNames = new List<string>();
        List<(string, string)> HeaderNames = new List<(string, string)>();
        List<(string, string)> SourceNames = new List<(string, string)>();
        List<(string, string)> OtherNames = new List<(string, string)>();

        internal VisualStudioCppItemsProjectGenerator()
        {
            projectDocument = new XmlDocument();
            filtersDocument = new XmlDocument();
        }

        internal void EnumerateFolder(
            string path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);

            FilterNames.Add(folder.FullName);

            foreach (var item in folder.GetDirectories())
            {
                EnumerateFolder(item.FullName);
            }

            foreach (var item in folder.GetFiles())
            {
                if (item.Extension == ".h" || item.Extension == ".hpp")
                {
                    HeaderNames.Add((item.FullName, item.Directory.FullName));
                }
                else if (item.Extension == ".c" || item.Extension == ".cpp")
                {
                    SourceNames.Add((item.FullName, item.Directory.FullName));
                }
                else
                {
                    OtherNames.Add((item.FullName, item.Directory.FullName));
                }
            }
        }

        internal XmlElement BuildFilterItemsFromFilterNames(
            List<string> FilterNames)
        {
            XmlElement FilterItems = filtersDocument.CreateElement(
                "ItemGroup",
                defaultNamespace);
            foreach (var FilterName in FilterNames)
            {
                XmlElement FilterItem = filtersDocument.CreateElement(
                    "Filter",
                    defaultNamespace);
                if (FilterItem != null)
                {
                    FilterItem.SetAttribute(
                        "Include",
                        FilterName);
                    XmlElement UniqueIdentifier = filtersDocument.CreateElement(
                        "UniqueIdentifier",
                        defaultNamespace);
                    if (UniqueIdentifier != null)
                    {
                        UniqueIdentifier.InnerText =
                            string.Format("{{{0}}}", Guid.NewGuid());
                        FilterItem.AppendChild(UniqueIdentifier);
                    }
                    FilterItems.AppendChild(FilterItem);
                }
            }

            return FilterItems;
        }

        internal (XmlElement, XmlElement) BuildItemsFromNames(
            string TypeName,
            List<(string, string)> Names)
        {
            XmlElement ProjectItems = projectDocument.CreateElement(
                "ItemGroup",
                defaultNamespace);
            XmlElement FiltersItems = filtersDocument.CreateElement(
                "ItemGroup",
                defaultNamespace);

            foreach (var Name in Names)
            {
                XmlElement ProjectItem = projectDocument.CreateElement(
                    TypeName,
                    defaultNamespace);
                if (ProjectItem != null)
                {
                    ProjectItem.SetAttribute(
                        "Include",
                        Name.Item1);
                    ProjectItems.AppendChild(ProjectItem);
                }

                XmlElement FiltersItem = filtersDocument.CreateElement(
                    TypeName,
                    defaultNamespace);
                if (FiltersItem != null)
                {
                    FiltersItem.SetAttribute(
                        "Include",
                        Name.Item1);
                    XmlElement Filter = filtersDocument.CreateElement(
                        "Filter",
                        defaultNamespace);
                    if (Filter != null)
                    {
                        Filter.InnerText = Name.Item2;
                        FiltersItem.AppendChild(Filter);
                    }
                    FiltersItems.AppendChild(FiltersItem);
                }
            }

            return (ProjectItems, FiltersItems);
        }

        internal void CreateFiles(
            string rootPath,
            string filePath,
            string fileName)
        {
            (XmlElement ProjectItems, XmlElement FiltersItems) HeaderItems =
                BuildItemsFromNames("ClInclude", HeaderNames);
            (XmlElement ProjectItems, XmlElement FiltersItems) SourceItems =
                BuildItemsFromNames("ClCompile", SourceNames);
            (XmlElement ProjectItems, XmlElement FiltersItems) OtherItems =
                BuildItemsFromNames("None", OtherNames);

            if (projectDocument != null)
            {
                projectDocument.InsertBefore(
                    projectDocument.CreateXmlDeclaration("1.0", "utf-8", null),
                    projectDocument.DocumentElement);

                XmlElement xmlElement = projectDocument.CreateElement(
                    "Project",
                    defaultNamespace);
                xmlElement.SetAttribute(
                    "ToolsVersion",
                    "4.0");

                xmlElement.AppendChild(HeaderItems.ProjectItems);
                xmlElement.AppendChild(SourceItems.ProjectItems);
                xmlElement.AppendChild(OtherItems.ProjectItems);

                projectDocument.AppendChild(xmlElement);

                projectDocument.InnerXml = projectDocument.InnerXml.Replace(
                    rootPath,
                    "");

                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.IndentChars = "  ";
                writerSettings.NewLineChars = "\r\n";
                writerSettings.NewLineHandling = NewLineHandling.Replace;
                writerSettings.Encoding = new UTF8Encoding(true);
                if (writerSettings != null)
                {
                    XmlWriter writer = XmlWriter.Create(
                       string.Format(
                           @"{0}\{1}.vcxitems",
                           filePath,
                           fileName),
                       writerSettings);
                    if (writer != null)
                    {
                        projectDocument.Save(writer);
                        writer.Flush();
                        writer.Dispose();
                    }

                }
            }

            if (filtersDocument != null)
            {
                filtersDocument.InsertBefore(
                    filtersDocument.CreateXmlDeclaration("1.0", "utf-8", null),
                    filtersDocument.DocumentElement);

                XmlElement xmlElement = filtersDocument.CreateElement(
                    "Project",
                    defaultNamespace);
                xmlElement.SetAttribute(
                    "ToolsVersion",
                    "4.0");

                xmlElement.AppendChild(
                    BuildFilterItemsFromFilterNames(FilterNames));
                xmlElement.AppendChild(HeaderItems.FiltersItems);
                xmlElement.AppendChild(SourceItems.FiltersItems);
                xmlElement.AppendChild(OtherItems.FiltersItems);

                filtersDocument.AppendChild(xmlElement);

                filtersDocument.InnerXml = filtersDocument.InnerXml.Replace(
                    rootPath,
                    "");

                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.IndentChars = "  ";
                writerSettings.NewLineChars = "\r\n";
                writerSettings.NewLineHandling = NewLineHandling.Replace;
                writerSettings.Encoding = new UTF8Encoding(true);
                if (writerSettings != null)
                {
                    XmlWriter writer = XmlWriter.Create(
                       string.Format(
                           @"{0}\{1}.vcxitems.filters",
                           filePath,
                           fileName),
                       writerSettings);
                    if (writer != null)
                    {
                        filtersDocument.Save(writer);
                        writer.Flush();
                        writer.Dispose();
                    }
                }
            }
        }

        public static void Generate(
            string inputFolder,
            string inputRootPath,
            string outputFilePath,
            string outputFileName)
        {
            VisualStudioCppItemsProjectGenerator generator =
                new VisualStudioCppItemsProjectGenerator();
            if (generator != null)
            {
                generator.EnumerateFolder(
                    inputFolder);
                generator.CreateFiles(
                    inputRootPath,
                    outputFilePath,
                    outputFileName);
            }
        }
    }
}
