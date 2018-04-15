// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open System.Net
open AngleSharp

module GetFileAndDirectoryInfo = begin
    let GetDirectory s =
        IO.Path.GetDirectoryName s

    let GetFileExtension s = 
        IO.Path.GetExtension s

    let GetFNameWithoutExtension s = 
        IO.Path.GetFileNameWithoutExtension s
end

type Rename() = 
    let mutable ext = ""
    let mutable dir = ""
    let mutable name = ""
    let mutable url = ""

    let baseurl = "https://commons.nicovideo.jp/material/"

    member this.SetParams s =
        ext <- GetFileAndDirectoryInfo.GetFileExtension s
        dir <- (GetFileAndDirectoryInfo.GetDirectory s) + "/"
        name <- GetFileAndDirectoryInfo.GetFNameWithoutExtension s
        url <- baseurl + name
    member this.DoRename =
        let config = Configuration.Default.WithDefaultLoader()

        let body = BrowsingContext.New(config).OpenAsync(url).Result.Body
    
        let selector = "div.commons_title"
        let cell = body.QuerySelectorAll(selector)

        let title = cell.Item(0).TextContent
        if title = null then
            printfn "Error: failed get data."
            exit 0

        let fname = dir + name + ext
        let movename = dir + name + "_" + title + ext
        printfn "Do rename %s to %s\n" fname movename
        IO.File.Move(fname, movename)

[<EntryPoint>]
let main argv = 
    if argv.Length < 1 then
        printfn "Please set args\n"
        exit 0

    let mutable l = []
    for i in argv do
        match i with
        | string -> l <- [i] |> List.append l

    let renamefunc = new Rename()
    for i in l do
        renamefunc.SetParams i
        renamefunc.DoRename

    0 // return an integer exit code
