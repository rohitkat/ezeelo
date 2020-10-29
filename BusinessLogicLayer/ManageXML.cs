//-----------------------------------------------------------------------
// <copyright file="CommonMethods.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Prashant Bhoyar</author>
// <DateTime> DEC 29, 2009 4:45 PM</DateTime>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;


namespace BusinessLogicLayer
{
    /// <summary>
    /// Determines whether to edit Connection node or Syntax node 
    /// </summary>
    public enum NodeOrChild
    {
        /// <summary>
        /// 
        /// Child: Specify Child Nodes to delete
        /// </summary>
        /// <param name="Node:"> Specify Node to delete</param>  
        Node = 1, Child = 2
    }

    public enum NodeAttributes
    {
        Null=0, Remove=1
    }

    public enum StyleSheetType
    {
        css=0, xsl=1
    }

    public enum Depth
    {
        AllChild=1,AllGrandChildren=2
    }

    public enum Direction
    {
        Parent=1,Child=2
    }
            
    public class MyXML
    {

        public static void CreateFile(string filePath,string rootNode,string styleSheetName,StyleSheetType type)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;


            if (type == StyleSheetType.css && !styleSheetName.ToUpper().Contains(".css".ToUpper()))
                styleSheetName += ".css";
            else if (type==StyleSheetType.xsl && !styleSheetName.ToUpper().Contains(".xsl".ToUpper()))
                styleSheetName += ".xsl";

            XmlWriter writer = XmlWriter.Create(filePath, settings);
            writer.WriteStartDocument();
                       
            writer.WriteRaw("\n");
            if(type==StyleSheetType.css)
               writer.WriteRaw("<?xml-stylesheet type=\"text/css\" href=\""+styleSheetName+"\"?>");
            else
               writer.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"" + styleSheetName + "\"?>");

            writer.WriteRaw("\n");
            
            writer.WriteStartElement(rootNode);
            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();
        }
        
        public static void NewFile(String filePath, String rootNode, List<string> attributeNames, List<string> attributeValues)
        {


            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            
            XmlWriter writer = XmlWriter.Create(filePath, settings);
            writer.WriteStartDocument();

            //Start creating elements and attributes
            writer.WriteStartElement(rootNode);

            //creating no of attributes which are in the list
            for (int i = 0; i < attributeNames.Count; i++)
                writer.WriteAttributeString(attributeNames[i], attributeValues[i]);


            writer.WriteEndElement();
            writer.WriteEndDocument();
            //clean up
            writer.Flush();
            writer.Close();

        }

        public static void NewFile(String filePath, String rootNode)
        {

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            XmlWriter writer = XmlWriter.Create(filePath, settings);
            writer.WriteStartDocument();

            //Start creating elements and attributes
            writer.WriteStartElement(rootNode);
            
            writer.WriteEndElement();
            writer.WriteEndDocument();
            //clean up
            writer.Flush();
            writer.Close();
 
        }

        public static bool IsNodeExist(String filePath, String xPath)
        {
            XmlDocument xDoc = new XmlDocument();

            try
            {
                xDoc.Load(filePath);
            }
            catch(Exception ex)
            {
                throw new Exception("File \"" + filePath + "\" Can't be loaded");
            }

            XmlNode xNode = xDoc.SelectSingleNode(xPath);
            
            if (xNode != null)
               return true;
            else
               return false;
            

        }

        public static bool IsFileExist(String filePath)
        {
            XmlDocument xDoc = new XmlDocument();

            try
            {
                xDoc.Load(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
       
        public static void DeleteNode(String filePath, String xPath,NodeOrChild deleteParam)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlNode childNode, parentNode;

            try
            {
                xDoc.Load(filePath);
            }
            catch { throw new Exception("Can't Open File \"" + filePath + "\""); }

            try
            {
                childNode = xDoc.SelectSingleNode(xPath);
                if(childNode==null)
                    throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");
            }
            catch { throw new Exception("Node to xPath \"" + xPath + "\" does not exist..."); }
            
           
            if (deleteParam == NodeOrChild.Node)
            {
                
                    parentNode = childNode.ParentNode;
                
                if (!parentNode.Name.Equals("#document"))
                    parentNode.RemoveChild(childNode);
                else
                    childNode.RemoveAll();
            }
            else
            {
                while (childNode.HasChildNodes)
                    childNode.RemoveChild(childNode.FirstChild);
                               
            }
            xDoc.Save(filePath);
        }

        public static void DeleteNode(String filePath, String xPath, NodeOrChild deleteParam, NodeAttributes attributeParam)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlNode childNode, parentNode;
            try
            {
                xDoc.Load(filePath);
            }
            catch { throw new Exception("Can't Open File \"" + filePath + "\""); }

            try
            {
                childNode = xDoc.SelectSingleNode(xPath);
                if (childNode == null)
                    throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");
            }
            catch { throw new Exception("Node to xPath \"" + xPath + "\" does not exist..."); }





            if (deleteParam == NodeOrChild.Node && attributeParam == NodeAttributes.Null)
            {
                while (childNode.HasChildNodes)
                    childNode.RemoveChild(childNode.FirstChild);
                
                if(childNode.Attributes!=null)
                    for (int i = 0; i < childNode.Attributes.Count; i++)
                        childNode.Attributes.Item(i).Value = "";

            }
            else if (deleteParam == NodeOrChild.Node && attributeParam == NodeAttributes.Remove)
                childNode.RemoveAll();
            else if (deleteParam == NodeOrChild.Child && attributeParam == NodeAttributes.Null)
            {
                Stack<XmlNode> pNodeToProcess = new Stack<XmlNode>();
                pNodeToProcess.Push(childNode);
                XmlNode xNode;
                while (pNodeToProcess.Count > 0)
                {
                    xNode = pNodeToProcess.Pop();

                    if (xNode.HasChildNodes)
                        foreach (XmlNode xmlNd in xNode.ChildNodes)
                            pNodeToProcess.Push(xmlNd);

                    if(xNode.Attributes!=null)
                        for (int i = 0; i < xNode.Attributes.Count; i++)
                            xNode.Attributes.Item(i).Value = "";

                }

            }
            else if (deleteParam == NodeOrChild.Child && attributeParam == NodeAttributes.Remove)
            {
                Stack<XmlNode> pNodeToProcess = new Stack<XmlNode>();
                pNodeToProcess.Push(childNode);
                XmlNode xNode;
                while (pNodeToProcess.Count > 0)
                {
                    xNode = pNodeToProcess.Pop();

                    if (xNode.HasChildNodes)
                        foreach (XmlNode xmlNd in xNode.ChildNodes)
                            pNodeToProcess.Push(xmlNd);
                    if(xNode.Attributes!=null)
                        for (int i = 0; i < xNode.Attributes.Count; i++)
                            xNode.Attributes.RemoveAll();
                }
            }
            else
                throw new Exception("Can't remove any Node or Attribute");
                   
            xDoc.Save(filePath);
        }

        public static void DeleteFile(String filePath)
        {
            try
            {
                System.IO.File.Delete(filePath);
            }
            catch { throw new Exception("Can't Delete File \""+filePath+"\"\nEither File does not exists or File is not accessible"); }
        }
       
        public static void InsertNode(String filePath, String xPath, String nodeName)
        {
            XmlDocument Doc = new XmlDocument();

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            XmlNode xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");

                       
            XmlElement childNode = Doc.CreateElement(nodeName);
            xNode.AppendChild(childNode);

            Doc.Save(filePath);


        }

        public static void InsertNode(String filePath, String xPath, String nodeName,int nodePosition)
        {
            XmlDocument Doc = new XmlDocument();

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            XmlNodeList xNodeList = Doc.SelectNodes(xPath);

            if (xNodeList == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");

            if(nodePosition>xNodeList.Count)
               nodePosition=xNodeList.Count-1;
            
            XmlNode xNode = xNodeList[nodePosition];

            XmlElement childNode = Doc.CreateElement(nodeName);
            xNode.AppendChild(childNode);

            Doc.Save(filePath);


        }
        
        public static void InsertNode(String filePath, String xPath, String nodeName,String innerText)
        {
            XmlDocument Doc = new XmlDocument();

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            XmlNode xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");


            XmlElement childNode = Doc.CreateElement(nodeName);
            xNode.AppendChild(childNode);

            if (!xNode.HasChildNodes)
                xNode.InnerText = innerText;
            else
                throw new Exception("Can Not set Inner Text as node is having Child nodes...");

            Doc.Save(filePath);


        }

        public static void InsertNode(String filePath, String xPath, String nodeName, String innerText, int nodePosition)
        {
            XmlDocument Doc = new XmlDocument();

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            XmlNodeList xNodeList = Doc.SelectNodes(xPath);

            if (xNodeList == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");



            if (nodePosition > xNodeList.Count)
                nodePosition = xNodeList.Count - 1;

            XmlNode xNode = xNodeList[nodePosition];


            XmlElement childNode = Doc.CreateElement(nodeName);
            xNode.AppendChild(childNode);

            if (!xNode.HasChildNodes)
                xNode.InnerText = innerText;
            else
                throw new Exception("Can Not set Inner Text as node is having Child nodes...");

            Doc.Save(filePath);


        }
               
        public static void InsertNode(String filePath, String xPath, String nodeName, List<string> attributeNames)
        {
            XmlDocument Doc = new XmlDocument();

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            XmlNode xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");


            XmlElement childNode = Doc.CreateElement(nodeName);
            xNode.AppendChild(childNode);

            //writes attributes values to the node
            for (int i = 0; i < attributeNames.Count; i++)
                childNode.SetAttribute(attributeNames[i], "");

            Doc.Save(filePath);


        }
               
        public static void InsertNode(String filePath, String xPath, String nodeName, List<string> attributeNames, List<string> attributeValues)
        {
            XmlDocument Doc = new XmlDocument();

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            XmlNode xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");


            XmlElement childNode = Doc.CreateElement(nodeName);
            xNode.AppendChild(childNode);

            //writes attributes values to the node
            for (int i = 0; i < attributeNames.Count; i++)
                   childNode.SetAttribute(attributeNames[i], attributeValues[i]);
            
            Doc.Save(filePath);


        }
               
        public static void InsertNodeAttributes(String filePath, String xPath, List<string> attributeNames, List<string> attributeValues)
        {
            XmlDocument Doc = new XmlDocument();

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
               throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            XmlNode xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");

            xNode.Attributes.RemoveAll();

            XmlAttribute[] attrib = new XmlAttribute[attributeNames.Count];

            //writes attributes to the node
            for (int i = 0; i < attributeNames.Count; i++)
            {
                attrib[i] = Doc.CreateAttribute(attributeNames[i]);
                attrib[i].Value = attributeValues[i];
                xNode.Attributes.Append(attrib[i]);
            }

            Doc.Save(filePath);

        }
        
        public static void InsertNodeAttributes(String filePath, String xPath, List<string> attributeValues)
        {
            XmlDocument Doc = new XmlDocument();

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            XmlNode xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");


            if (xNode.Attributes == null)
                throw new Exception("Attributes to node at xPath \"" + xPath + "\" does not exists...");            
            else if( xNode.Attributes.Count != attributeValues.Count)
                throw new Exception("No of attribute values Provided does not match with no of attributes of the node at given xPath");    

            
            //writes attributes values to the node
            for (int i = 0; i < attributeValues.Count; i++)
                 xNode.Attributes.Item(i).Value = attributeValues[i];
            
            Doc.Save(filePath);

        }

        public static void SetAttribute(String filePath, String xPath,string attributeName, string attributeValue)
        {
            XmlDocument Doc = new XmlDocument();

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            XmlNode xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");


            if (xNode.Attributes == null)
                throw new Exception("Attributes to node at xPath \"" + xPath + "\" does not exists...");
            

            //writes attributes values to the node

            int i=0;
            for (; i < xNode.Attributes.Count && !xNode.Attributes.Item(i).Name.Equals(attributeName); i++) ;

            if (i != 0)
                xNode.Attributes.Item(i).Value = attributeValue;
            else
                throw new Exception("Attributes to node at xPath \"" + xPath + "\" does not exists...");


            Doc.Save(filePath);

        }
               
        public static void GetAttributes(String filePath,String xPath,ref Dictionary<String,String> attributes)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath);

            XmlNode xNode = xDoc.SelectSingleNode(xPath);

            for (int i = 0; i < xNode.Attributes.Count; i++)
                    attributes.Add(xNode.Attributes.Item(i).Name,xNode.Attributes.Item(i).Value);

                                   
        }

        public static void GetAttributes(String filePath, String xPath, ref Dictionary<String, String> attributes, Direction direction, int depth)
        {

            XmlDocument Doc = new XmlDocument();
            XmlNode xNode;

            try
            {
                Doc.Load(filePath);

            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");

            Stack<XmlNode> childStack = new Stack<XmlNode>();
            childStack.Push(xNode);

            if (direction == Direction.Parent)
                for (int i = 0; i < depth; i++)
                    if (xNode.ParentNode.Name.Equals("#document"))
                        throw new Exception("Parent Node is not present at this level");
                    else
                        xNode = xNode.ParentNode;
            else
                for (int i = 0; i < depth; )
                {
                    xNode = childStack.Pop();
                    if (xNode.HasChildNodes)
                    {
                        for (int j = 1; j < xNode.ChildNodes.Count; j++)
                            if(!xNode.ChildNodes[j].Name.Equals("#text"))
                            childStack.Push(xNode.ChildNodes[j]);
                        xNode = xNode.FirstChild;
                        i++;
                    }
                    else if (childStack.Count != 0)
                    {
                        xNode = childStack.Pop();
                        i--;
                    }
                    else
                    {
                        throw new Exception("Child Node is not present at this level");
                    }

                }

            for (int i = 0; i < xNode.Attributes.Count; i++)
                attributes.Add(xNode.Attributes.Item(i).Name, xNode.Attributes.Item(i).Value);

        }

        public static void GetAttributes(String filePath, String xPath, ref Dictionary<String, String> attributes, string parentName)
        {

            XmlDocument Doc = new XmlDocument();
            XmlNode xNode;

            try
            {
                Doc.Load(filePath);

            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");

            while (!xNode.Name.Equals(parentName))
            {
                if (xNode.ParentNode.Name.Equals("#document"))
                    throw new Exception("Parent \"" + parentName + "\" does not exists...");
                xNode = xNode.ParentNode;
            }

            for (int i = 0; i < xNode.Attributes.Count; i++)
                attributes.Add(xNode.Attributes.Item(i).Name, xNode.Attributes.Item(i).Value);


        }
               
        public static string GetSingleAttribute(String filePath, String xPath,string attributeName)
        {
           
            XmlDocument Doc = new XmlDocument();
            XmlNode xNode;
           
            try
            {
                
                Doc.Load(filePath);

            }
            catch(Exception ex)
            {
                throw new Exception("Can't Open File \"" + filePath + "\"\nException:"+ex.Message);
            }


            xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");


            foreach (XmlAttribute att in xNode.Attributes)
                if (att.Name.Equals(attributeName))
                    return att.Value;
                
            return string.Empty;
         
        }

        public static string GetSingleAttribute(String filePath, String xPath, string attributeName, Direction direction, int depth)
        {
           
            XmlDocument Doc = new XmlDocument();
            XmlNode xNode;
           
            try
            {
                Doc.Load(filePath);

            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");

            Stack<XmlNode> childStack = new Stack<XmlNode>();
            childStack.Push(xNode);

            int level = 0;

            if (direction == Direction.Parent)
                for (int i = 0; i < depth; i++)
                    if (xNode.ParentNode.Name.Equals("#document"))
                        throw new Exception("Parent Node is not present at this level");
                    else
                        xNode = xNode.ParentNode;
            else
                for (int i = 0; i < depth; )
                {
                    xNode = childStack.Pop();
                    if (xNode.HasChildNodes)
                    {
                        for (int j = 1; j < xNode.ChildNodes.Count; j++)
                            if(!xNode.ChildNodes[j].Name.Equals("#text"))
                            childStack.Push(xNode.ChildNodes[j]);
                        xNode = xNode.FirstChild;
                        i++;
                    }
                    else if (childStack.Count != 0)
                    {
                        xNode = childStack.Pop();
                        i--;
                    }
                    else
                    {
                        throw new Exception("Child Node is not present at this level");
                    }

                }


            foreach (XmlAttribute att in xNode.Attributes)
                if (att.Name.Equals(attributeName))
                    return att.Value;

            return string.Empty;



        }

        public static string GetSingleAttribute(String filePath, String xPath, string attributeName, string parentName)
        {
           
            XmlDocument Doc = new XmlDocument();
            XmlNode xNode;
            
            try
            {
                Doc.Load(filePath);

            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");

            while (!xNode.Name.Equals(parentName))
            {
                if (xNode.ParentNode.Name.Equals("#document"))
                    throw new Exception("Parent \"" + parentName + "\" does not exists...");
                xNode = xNode.ParentNode;
            }

            foreach (XmlAttribute att in xNode.Attributes)
                if (att.Name.Equals(attributeName))
                    return att.Value;

            return string.Empty;



        }
        
        public static string[] GetAllChildAttributes(String filePath, String xPath, string attributeNameToSearch)
        {

            XmlDocument Doc = new XmlDocument();
            XmlNode xNode;

            try
            {
                Doc.Load(filePath);

            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");


            string[] attrib = new string[xNode.ChildNodes.Count];
            
            int i=0;
            foreach (XmlNode child in xNode.ChildNodes)
            {
                foreach (XmlAttribute att in child.Attributes)
                    if (att.Name.Equals(attributeNameToSearch))
                        attrib[i++] = att.Value;
            }
            
            return attrib;

        }

        public static List<string> GetAllChildAttributes(String filePath, String xPath, string attributeNameToSearch,Depth depth)
        {

            XmlDocument Doc = new XmlDocument();
            XmlNode xNode;

            try
            {
                Doc.Load(filePath);

            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");


            Stack<XmlNode> currentNode = new Stack<XmlNode>();
            currentNode.Push(xNode);

            List<string> attValues = new List<string>();

            if (depth == Depth.AllChild)
            {
                foreach (XmlNode child in xNode.ChildNodes)
                {
                    foreach (XmlAttribute att in child.Attributes)
                        if (att.Name.Equals(attributeNameToSearch))
                            attValues.Add(att.Value);
                }

                return attValues;
 
            }

            while (currentNode.Count != 0)
            {
                xNode = currentNode.Pop();
               
                foreach (XmlNode node in xNode.ChildNodes)
                    currentNode.Push(node);

               
                foreach (XmlNode child in xNode.ChildNodes)
                {
                    if(!child.Name.ToUpper().Equals("#text".ToUpper()))
                    foreach (XmlAttribute att in child.Attributes)
                        if (att.Name.Equals(attributeNameToSearch))
                             attValues.Add(att.Value);
                }
            }

            return attValues;

        }

        public static string[] GetAllNodeAttributes(String filePath, String xPath, string attributeNameToSearch)
        {

            XmlDocument Doc = new XmlDocument();
            XmlNodeList xNodeList;

            try
            {
                Doc.Load(filePath);

            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            xNodeList = Doc.SelectNodes(xPath);

            if (xNodeList == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");


            string[] attrib = new string[xNodeList.Count];

            int i = 0;

            foreach (XmlNode xNode in xNodeList)
            {
                foreach (XmlAttribute att in xNode.Attributes)
                    if (att.Name.Equals(attributeNameToSearch))
                        attrib[i++] = att.Value;
            }

            return attrib;

        }

        public static string GetInnerText(String filePath, String xPath)
        {

            XmlDocument xDoc = new XmlDocument();
            XmlNode xNode;

            try
            {
                xDoc.Load(filePath);
            }
            catch
            {
                throw new Exception("File \"" + filePath + "\" Can't be loaded");
            }

            try
            {
                xNode = xDoc.SelectSingleNode(xPath);

            }
            catch
            {
                throw new Exception("Node Not Found At xPath \"" + xPath + "\"");
            }

            return xNode.InnerText;



        }

        public static void SetInnerText(String filePath, String xPath,string innerText)
        {

            XmlDocument xDoc = new XmlDocument();
            XmlNode xNode;

            try
            {
                xDoc.Load(filePath);
            }
            catch
            {
                throw new Exception("File \"" + filePath + "\" Can't be loaded");
            }

            try
            {
                xNode = xDoc.SelectSingleNode(xPath);

            }
            catch
            {
                throw new Exception("Node Not Found At xPath \"" + xPath + "\"");
            }

            xNode.InnerText = innerText;

            xDoc.Save(filePath);



        }
        
        public static int GetCount(String filePath, String xPath, Depth depth)
        {

            XmlDocument Doc = new XmlDocument();
            XmlNode xNode;

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");

            Stack<XmlNode> currentNode = new Stack<XmlNode>();
            currentNode.Push(xNode);

            if (depth == Depth.AllChild)
                return xNode.ChildNodes.Count;


            int count = 0;
            
            while (currentNode.Count != 0)
            {
                xNode = currentNode.Pop();
                count += xNode.ChildNodes.Count;
                
                foreach (XmlNode node in xNode.ChildNodes)
                    currentNode.Push(node);
            }

            return count;

 
        }

        public static bool IsCountExists(String filePath, String xPath,int number)
        {

            XmlDocument Doc = new XmlDocument();
            XmlNode xNode;

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            xNode = Doc.SelectSingleNode(xPath);

            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");

           
            foreach (XmlNode nod in xNode.ChildNodes)
                if (nod.ChildNodes.Count == number)
                    return true;

            return false;


        }

        public static bool IsAttributeExists(String filePath, String xPath,String attributeName,String itemToSearch)
        {
            XmlDocument Doc = new XmlDocument();
            XmlNode xNode;

            try
            {
                Doc.Load(filePath);
            }
            catch
            {
                throw new Exception("Can't Open File \"" + filePath + "\"");
            }


            xNode = Doc.SelectSingleNode(xPath);
            
            if (xNode == null)
                throw new Exception("Node to xPath \"" + xPath + "\" does not exist...");




            Stack<XmlNode> currentNode = new Stack<XmlNode>();
            currentNode.Push(xNode);

            
            while (currentNode.Count != 0)
            {
                xNode = currentNode.Pop();
                //count += xNode.ChildNodes.Count;

                foreach (XmlNode node in xNode.ChildNodes)
                {
                    foreach(XmlAttribute att in xNode.Attributes)
                        if(att.Name.Equals(attributeName ) && att.Value.Equals(itemToSearch))
                            return true;

                    currentNode.Push(node);
                }
            }


            return false;

             

        }
     

    }
}
