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

To get started with Enterspeedm you should start reading the, __[getting started](https://docs.enterspeed.com/getting-started)__ guide, as well as reading about the key concepts of Enterspeed. 

[Schemas](https://docs.enterspeed.com/key-concepts/schemas)
[Partial schemas](https://docs.enterspeed.com/key-concepts/schemas)
[Views](https://docs.enterspeed.com/key-concepts/schemas)
[Data sources](https://docs.enterspeed.com/key-concepts/schemas)

### Migrator

The migrator is software that should be considered as the layer between the output of Enterspeed and the target CMS. The migrator consists of a generic library that reads the output of Enterspeed, and converts this output into usable .NET objects. We can use this generic layer for multiple target CMS, since its only role is to convert the output from Enterspeed into usable .NET objects. 

We currently have one data conversion layer that consumes the output of the generic library and converts it into Umbraco data. This layer is very extendable and you will be able to handle your custom data types and complex objects here. 


## Migrator in details
### Architecture
#### High-level technical description of the architecture
### Generic migration engine
#### Extendability
### CMS specific data converter
#### Dependencies 
#### Extendability

## Getting started
### SOP (Describing getting started todos in broad terms)
### Describing migrator-specific work. The description should reference technical-specific details described in "Migrator in details"
### Caveats and todos