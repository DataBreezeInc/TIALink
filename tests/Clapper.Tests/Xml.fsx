open System.Xml
open System.Xml.Linq
open System.IO
open System.Text

type Xml =
    | Element of string * (string* string) seq * string * Xml seq
    member this.WriteContentTo(writer:XmlWriter) =
        let rec write element =
            match element with
            | Element (name, attributes, value, children) ->
                writer.WriteStartElement(name)
                for a in attributes do
                    writer.WriteAttributeString(fst a, snd a)
                writer.WriteString(value)
                children |> Seq.iter write
                writer.WriteEndElement()
        write this
    override this.ToString() =
        let output = StringBuilder()
        using (new XmlTextWriter(new StringWriter(output),
                Formatting=Formatting.Indented))
            this.WriteContentTo
        output.ToString()

// let xmlNs           = "somenamespaceurl"
// let XNameStd   name = XName.Get(name, xmlNs)

// let InsertIfCondition valueToInsert valueToCheck attributeName (element:XElement) =
//   if valueToInsert = valueToCheck then
//     element.Add(XAttribute(XNameStd attributeName,valueToInsert))


// let InsertSthToTree(sth:DomainObject) (currentRoot:XElement) =
//   let element = XElement(XNameStd "MyDomainElementInXML")
//   currentRoot.Add(element)
//   InsertIfCondition(sth.BooleanProperty,true,"IsEnabled",element)

// let CreateXMLDoc =
//   let someObj = DomainObject(...)
//   let doc     = XDocument()
//   InsertSthToTree(doc.Root)
//   doc
let documentInfo =
    Element("DocumentInfo",[],"",[
            Element("dokumentinstanz",[("kennung","Teil_A")],"",[
                // Element("datum",[],date(),[])
                // Element("uhrzeit",[],time(),[])
                Element("anwendung",[],"",[
                    Element("anwendungsname",[],"EnergyReport",[])
                    Element("version",[],"0.0.1",[])
                    Element("hersteller",[],"Danpower GmbH",[])
                ])
            ])
        ])
let engineeringVersionElement = XElement("Engineering", "version=V17")
let documentElement = XElement("Document",engineeringVersionElement)
let doc =  XDocument()
doc.Add(documentElement)
doc.Save(Path.GetFullPath("./Test.xml"))