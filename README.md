# producetool
This producetool is used for four item:
1. git extensions
2. produce(work item-->pull request-->review-->checkin)
3. simple web crawler
4. azure analysis

# Git Extensions
Inputing ```albert -git info``` is equivalent to execute combined command.
```
1.cd ..
2.git add .
3.git commit -m info
4.git push
```

# Process
- Get the DLL name from WorkItem
- Use [Tool](http://10.158.22.18/#/QueryAssemblyDetail) to Analyze the source path of dll.
- Start windows terminal
- cd source path
- cd ..
- dir-->Check if there is a folder with dotNetcore Version
- cd source path(If there is no existence, enter the source code directory)
- produce netcore
- msbuild -t:restore-->NoError-->bcc-->NoError-->update DotNetCoreMigration/DotNetCoreMigrationAlignment.md
- root-->git status-->git add .-->git commit -m "Produce xxx.dll"-->git push
- Go to [PR](https://o365exchange.visualstudio.com/O365%20Core/_workitems/edit/1780212/), Create a pull request.
- Wait for review & Focus
- Solve some question and comments
- Update [OneNote](https://microsoftapc-my.sharepoint.com/:o:/r/personal/v-texu_microsoft_com/_layouts/15/Doc.aspx?sourcedoc=%7Bd42fa092-9b22-4fb3-b194-0176df355d83%7D&action=edit&wd=target(Produce.one%7CD63373F8-D85E-4ACB-A1AD-F7EF27486E29%2FMapiHttp%20Produce%20for%202021-Aug-Sprint8%7Cee2ac6c6-8a84-428e-a45b-08d143a646d5%2F)) state 
- Update [Tool](http://10.158.22.18/#/QueryAssemblyDetail) state
- Done

# Simple web crawler
Inputing ```albert -crawl``` can crawl all information of the website configured by Producetool.json(PersonalCrawlingSite)