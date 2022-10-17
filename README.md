# Enterspeed migrator

## Concept
The migration tools consist of 3 things. A source CMS, Enterspeed, and a target CMS that we are migrating data to. Enterspeed gives us the flexibility to form the data structure to our specific needs and essentially decouples the data architecture from the source CMS. 

We have been working on some concepts utilizing this flexibility to make a migration from an old version of a CMS to a new version. Due to the data being decoupled, there is an added bonus of being able to migrate from one system to another, since we have control over the delivery structure and output from Enterspeed.

### Purpose
This is a framework for rapid data migration between systems. It is a time-consuming and expensive process to upgrade your old system to the newest version. We provide you with tools to get started migrating your data fast, instead of upgrading your old system. 

## High-level overview
The migration tool consists of multiple parts. We will dive deeper into these here, for you to understand the working parts and give you an idea of how to work with the migration tool. 

### Source CMS
The source CMS is the system that you want to push your data to Enterspeed from. 
You will need a way to ingest data into Enterspeed from your source system. Luckily this is already made easy for you, with some great documentation on the ingest API. We also have some premade **connectors**, so you can get started immediately.

When you have set a connection up between your source CMS and Enterspeed, you can start ingesting data into Enterspeed.

### Enterspeed
The data now exists as [source entities](https://docs.enterspeed.com/key-concepts/schemas) in Enterspeed and can be formed and modeled easily with data mapping in Enterspeed schemas. The schemas define the api endpoint and data structure for your data. This is where the decoupling is happening, since you are specifying the new structure of your data through schema mappings, and generating views based on the mappings. 

It is important to understand that we are not seeing the output of the source entities, but generating a [view](https://docs.enterspeed.com/key-concepts/schemas) that is based on the schema mapping. The source entities are the raw data, the schemas are where we define how we want the output, and [views](https://docs.enterspeed.com/key-concepts/schemas) are the output itself. 

To get started with Enterspeed you should start reading the __[getting started](https://docs.enterspeed.com/getting-started)__ guide, as well as reading about the key concepts of Enterspeed. 

[Schemas](https://docs.enterspeed.com/key-concepts/schemas)
[Partial schemas](https://docs.enterspeed.com/key-concepts/schemas)
[Views](https://docs.enterspeed.com/key-concepts/schemas)
[Data sources](https://docs.enterspeed.com/key-concepts/schemas)

### Migrator

The migrator is software that should be considered as the layer between the output of Enterspeed and the target CMS.


## Migrator

The migrator solution consists of two layers. The __`Enterspeed.Migrator`__ layer is the generic layer that calls the Enterspeed API and turns the response into usable structured data in .NET objects. 
This response is then used by the CMS-specific converter. In this example, we are showing the __`Umbraco10.Migrator`__ project. This layer interprets the objects we receive from the generic layer and converts them into Umbraco-specific types and saves them in the target CMS system. 

![migrator-solution](/assets/migrator-solution_cmeg05yrn.png)

### Generic migration engine
The generic business logic is confined to three different services. The APIService, PageResolver and SchemaBuilders. 

![migrator-solution](/assets/generic-migrator.png)

The APIService has the responsibility of establishing a connection to Enterspeed. 
It contains 2 public methods. 
- `GetNavigationAsyc`
- `GetPageResponsesAsync`

`GetNavigationAsync` calls a [handle](https://docs.enterspeed.com/key-concepts/schemas) that you have set up. This handle returns a list of routable urls in a tree structure. 
This response has to conform to a specific format. You can get the example schema setup for the handle __[here](https://docs.enterspeed.com/key-concepts/schemas)__. 

`GetPageResponsesAsync` Returns a list of responses. It calls all the URLs from the navigation data, and converts them into PageResponse objects, which wraps the delivery api response type, and also allows for page responses to contain children. Thereby keeping a 1-to-1 relation to the page structure that we are getting from Enterspeed.

Example of the PageData objects returned from GetPageResponsesAsync
```cs title="PageData object"
public class PageResponse
{
    public DeliveryApiResponse DeliveryApiResponse { get; set; }
    public List<PageResponse> Children { get; set; }
}
```


The `PageResolver` takes the page responses and converts them to page data objects. Below is the types used in the page data objects.

```cs title="PageData object"
public class PageData
{
    public MetaSchema MetaSchema { get; set; }
    public List<EnterspeedPropertyType> Properties { get; set; }
    public List<PageData> Children { get; set; }
}
```

```cs title="EnterspeedPropertyType"
public class EnterspeedPropertyType
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public JsonValueKind @Type { get; set; }
    public object Value { get; set; }
    public List<EnterspeedPropertyType> ChildProperties { get; set; }
}
```

As you can see we are allowing this object to contain children of itself. This is to mimic a content tree structure. 
We also have the property types defined here as well. This setup allows multiple nested complex types, and can therefore support nested arrays, objects and much more. These are all mapped recursively.

The `SchemaBuilder` converts all page data into a set of unique objects. The schemas are the sum of all the data and types that were received in the earlier steps. It has figured out how many property types Schema xx should have, their names as well as their value types. A schema is best compared to a document type in Umbraco.

```cs title="Schema"
public class Schema
{
    public MetaSchema MetaSchema { get; set; }
    public List<EnterspeedPropertyType> Properties { get; set; }
}
```

So now we have a clean set of data, with all its unique schemas that we can work with. This is here the CMS-specific conversion starts.

### CMS-specific data converter





## Getting started
### SOP (Describing getting started todos in broad terms)
### Describing migrator-specific work. The description should reference technical-specific details described in "Migrator in details"
### Caveats and todos