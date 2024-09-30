# Dotnet Todo.txt CLI

[![Build Status](https://github.com/rprouse/dotnet-todo/actions/workflows/dotnet-core.yml/badge.svg)](https://github.com/rprouse/dotnet-todo/actions/workflows/dotnet-core.yml) ![NuGet Downloads](https://img.shields.io/nuget/dt/dotnet-todo)

`dotnet-todo` is a .NET command line port of [Todo.txt](http://todotxt.org/) that tries to
remain faithful to the command line and functionality of the original shell script wherever
possible. As such, the [usage](#usage) below is a modified copy of the
[original on GitHub](https://github.com/todotxt/todo.txt-cli/blob/master/USAGE.md).

- [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)

## Installation

This program is a [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) and requires the latest version of
the [.NET SDK](https://dotnet.microsoft.com/download) to be installed. .NET 8.0 or newer is required.

### Enabling Tab Completion

This program supports tab completion using `dotnet-suggest`. To enable, for each shell
you must install the `dotnet-suggest` global tool and adding a shim to your profile. This
only needs to be done once and work for all applications built using `System.CommandLine`.

Follow the [setup instructions](https://github.com/dotnet/command-line-api/blob/main/docs/dotnet-suggest.md)
for your shell.

## Usage

```shell
todo [-fhpantvV] [-d todo_config] action [task_number] [task_description]
```

For a complete list of options,

```sh
todo --help
```

### Warning

The Windows command line uses the `@` sign to indicate that command line arguments should be loaded from
the file after the @ sign. This is a problem when searching for a context in your task list which also uses
the command line. For example, if I wanted to search for my tasks with `@work`, I would normally try this and
get the following error;

```shell
> todo list @work
Response file not found 'work'
```

In Powershell, the above command does not filter on the context and returns all items. If you try to add
quotes, `todo list "@work"` in Powershell, then you get the same error.

I have not found a way to escape the `@` on the command line and adding quotes does not work. As a workaround
you can leave out the `@` sign and search using `todo list work`. This will also include tasks with the word
work, but that is minor. I may add `listcon` and `listpri` commands in the future.

If anyone has a proper workaround, please file an issue and I will update this.

Also note, that unlike the shell script version, quotes are required around any strings with spaces.

## Configuration

This program does **not** support the default `todo.cfg` file as provided
by the original shell script. Instead, this program uses a JSON file called
`~/.todo.json` in the user's home directory.

The defaults for this file put the todo files in a `Todo` directory in the
users `Documents` directory.

Allowed colors are `black, blue, cyan, gray, green, magenta, red, white,
yellow, darkBlue, darkCyan, darkGray, darkGreen, darkMagenta, darkRed,
darkYellow`. Either omit or set any color to `null` to use the default
terminal color.

You only need to add lines to `~/.todo.json` that you want to change.
Everything else will be set to the defaults listed below.

The format and defaults for this file are;

```json
{
  "todoDirectory": "C:\\Users\\username\\Documents\\Todo",
  "todoFile": "Todo.txt",
  "doneFile": "Done.txt",
  "reportFile": "Report.txt",
  // Colors for each priority from A to Z
  "priorities": {
    "A": {
      "color": "yellow",
      "backgroundColor": null
    },
    "B": {
      "color": "green",
      "backgroundColor": null
    },
    "C": {
      "color": "cyan",
      "backgroundColor": null
    }
  },
  // Color of done items
  "doneColor": {
    "color": "darkGray",
    "backgroundColor": null
  },
  // Color of any +projects within the text
  "projectColor": {
    "color": "red",
    "backgroundColor": null
  },
  // Color of any @contexts within the text
  "contextColor": {
    "color": "red",
    "backgroundColor": null
  },
  // Color of dates like 2020-10-22
  "dateColor": {
    "color": "magenta",
    "backgroundColor": null
  },
  // Color of the task numbers
  "numberColor": {
    "color": "gray",
    "backgroundColor": null
  },
  // The color of name value pairs like DUE:2020-10-22
  "metaColor": {
    "color": "darkCyan",
    "backgroundColor": null
  }
}
```

Here is very simple example, just changing the directory that your todo
files are stored in and using the defaults for everything else. Note
that the trailing slashes on the directory are optional.

```json
{
  "todoDirectory": "G:\\My Drive\\todo\\"
}
```
