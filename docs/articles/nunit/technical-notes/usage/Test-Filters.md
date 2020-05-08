Test Filters represent a selection of tests to be displayed, run or loaded. When a filter needs to be passed to the framework, it is passed as a string containing an XML fragment. This page describes the elements present in the XML for a filter.

#### `<filter>`

This is the required top-level element for any filter. If it contains no other elements, it represents an empty filter. If it contains just one element, that element is used as the filter for all tests. If it contains multiple elements, it works like an `<and>` element.

Child elements allowed: `<and>`, `<or>`, `<not>`, `<id>`, `<test>`, `<cat>`, `<class>`, `<method>`, `<namespace>`, `<prop>`, `<name>`.

#### `<and>`

Represents an AndFilter. All contained filter elements must pass in order for this filter to pass.

Child elements allowed: `<and>`, `<or>`, `<not>`, `<id>`, `<test>`, `<cat>`, `<class>`, `<method>`, `<namespace>`, `<prop>`, `<name>`.

#### `<or>`

Represents an OrFilter. At least one contained filter element must pass in order for this filter to pass.

Child elements allowed: `<and>`, `<or>`, `<not>`, `<id>`, `<test>`, `<cat>`, `<class>`, `<method>`, `<namespace>`, `<prop>`, `<name>`.

#### `<not>`

Represents a NotFilter. The single contained filter element must fail in order for this filter to pass.

Child elements allowed: `<and>`, `<or>`, `<not>`, `<id>`, `<test>`, `<cat>`, `<class>`, `<method>`, `<namespace>`, `<prop>`, `<name>`.

#### `<id>`

Represents an IdFilter. The text of the element contains a single test id or multiple ids separated by commas. Since test ids are an internal construct, this filter is not useful externally. However, it is used by the NUnit Gui to efficiently select tests from the list of those loaded.

Child elements allowed: None.

#### `<test>`

Represents a selection by test name. The full name of the test is used as its inner text.
If the filter should use a regular expression for matching then the element should contain an attribute named `re` with the value `"1"`.

Child elements allowed: None.

#### `<cat>`

Represents a CategoryFilter. The text of the element contains a single category.
If the filter should use a regular expression for matching then the element should contain an attribute named `re` with the value `"1"`.

Child elements allowed: None.

#### `<class>`

Represents a selection by class name. The fully qualified name of the class is used as its inner text. 
If the filter should use a regular expression for matching then the element should contain an attribute named `re` with the value `"1"`.

Child elements allowed: None.

#### `<method>`

Represents a selection by method name. The name of the method is used as its inner text.
If the filter should use a regular expression for matching then the element should contain an attribute named `re` with the value `"1"`.

Child elements allowed: None.

#### `<namespace>`

Represents a selection by namespace. The fully qualified name of the namespace is used as its inner text.
If the filter should use a regular expression for matching then the element should contain an attribute named `re` with the value `"1"`.

Child elements allowed: None.

#### `<prop>`

Represents a PropertyFilter. The element has an attribute `name` which specifies the name of the property.
The text of the element contains the value of the property.
If the filter should use a regular expression for matching then the element should contain an attribute named `re` with the value `"1"`.

Child elements allowed: None.

#### `<name>`

Represents a selection by test name. The test name assigned by NUnit is used as its inner text.
If the filter should use a regular expression for matching then the element should contain an attribute named `re` with the value `"1"`.

Child elements allowed: None.
