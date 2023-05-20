# Fatum

Mid-Serialization/Deserialization Metadata Library

## What is "Mid-Serialization"?

These are the steps that exist between Parsing and Serialization for any metadata structure.

## Reason for the Fatum Library

Because data may change but your strictly typecast serialized data does not, resulting in failures and losses of data when the data is not only good, but probably still evolving.  Adhering to a data contract is admirable, but realistically it stymes discovery.

Also, we don't exist in a world where this actually makes sense for AI or any other form of critical thinking.  For example, we may understand a rock really well, but if we encounter a new type of mineral it doesn't necessary subtract from the total properties rocks in general, it adds to it.  If the model ignores this new information, we just lost great and wonderful knowledge.

Fatum is THE most central library in the Proliferation Project, with Proliferation Flows (data in motion solution) and Proliferation Absolution (forensic solution) being the two main consumers.  It enables the ability to make hot swap modifications to the data intake (i.e., not compiled like a DTO) and ability to capture and retain data that normally would be discarded.

## Origins Fatum

Fatum was created to handle describing computer security events.  Security control #1 might be an intrusion detection system that will tell if a computer is attacked.  Security control #2 might be a host-based intrusion detection system like "Tripwire".  Security control #3 might be a firewall.  Security Control #4 might be an anti-virus.

- Some of these controls have a remote host identification (Firewall, Intrusion Detection)
- Some of these controls have a fire identification (HIDS, Anti-Virus)
- Some of these controls will have an action performed (Firewall, IDS, and Anti-Virus)
- Some of these controls will produce detailed data (Anti-Virus)
- All of these controls have lots of common competators in the marketplace that use different metadata structures
- Most of these controls have frequent updates and can change their metadata structures at any time.

After having added support for hundreds of devices for half a dozen different SIEM tools, it became quickly clear that one size fits all doesn't exist.  After the security industry tried and failed miserably to adopt a single logging format (CEF, syslog, Microsoft Events, blah blah blah.) it became clear that in order to provide an analysis engine that handled everything reliably that it was fed, it needed to far more dynamic than any of the existing tools were capable of.

Two solutions were on the table:  first is performing analysis for data at rest (i.e., "Big Data") solutions. Very expensive success was achieved here, and a lot of in-house failures.  Second is performing analysis while the data is still in motion, which often relied on a **vendor** to provide more resources then they would want to in order to manage.  (As opposed to a community and/or company that is consuming the product.)

To optimize, the following steps are suggested:

1) Collect data
2) Mid-Deserialize data <--- Fatum's Key Contribution
3) Analyze data in motion
4) Storage
5) Forward (note that the forwarding can be straight back into another workstream at stage #3)

## Okay, sold. How?

Metadata is represented typically in two forms:  array of objects and a tree of objects.  A tree of object implies an array of objects, so the core of Fatum is a simple Tree structure.

Object is represented typically in four forms:  a binary array, an integer value (signed/unsigned) of X * bytes size, a floating point value (signed/unsigned) of X * bytes size, and a string.  As an observation, all values can be represented as a string, and so the lowest common denominator for data representation for this tree is and always will be a string.

This doesn't mean that strict typecasting isn't possible, and but "under the circumstances" having it doesn't add nearly as much value as we'd expect:

1)  Metadata can be used for strict typecasting
2)  Fatum allows the possibility that a field name can be both string or numeric.
3)  Fatum doesn't target data-at-rest, it targets Data-In-Memory and Data-In-Motion.
4)  Thanks to most modern API design and all that tossing of JSON/XML data across networks, we're parsing this stuff anyway.

## Anything else?

With all data coming into a solution that uses Fatum such as Proliferation Flows or Proliferation Absolution, several basic functions become universal such as Auto-Schema, Tree Merge, Pivots, etc.  All of these manipulations are included as part of the Fatum Library.

## Examples

> Tree simpleTree = new Tree();                                              // Create a Tree structure that is blank.
> Tree parsedTree = TeeDataAccess.readXMLFromString(<xml data in string>)    // Parse an XML structure
> Tree fileTree = new Tree("filename")                                       // Create a Tree from a file containing parsable metadata
> 
> Tree childTree = new Tree();                                               // Create a child tree and add it to the parent
> simpleTree.AddNode(childTree);
>
> string xmlForm = TreeDataAccess.writeTreeToXMLString(xmlForm,"Root")       // Creates an XML structure representing contents of simpleTree
> TreeDataAccess.writeJson(filename, simpleTree, "root");                    // Writes tree to a file in JSON format
>  ...
> string value = parsedTree.GetElement("Property");                          // Gets the value of a property in the immediate note
> Tree childNode = parsedTree.findNode("Leafname");                          // Returns a tree or null value for the provided leaf name
