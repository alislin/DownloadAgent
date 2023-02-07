# DownloadAgent
Download tools
下载服务

### 编译

```ps
.\publish.bat
```

### 运行
```ps
.\DownloadAgent.exe
```
启动后，查看下载列表和进度，设置保存路径  
http://localhost:36520/index.html

## 项目说明
### 下载列表页项目
项目在 `web` 目录，使用 `svelte` 构建

### 服务Api
运行后，可通过 [Api文档](http://localhost:36520/swagger/index.html)  查看

- 单个文件下载
    ```json
    // [post] /api/Manage/download/item
    {
    "url": "string",
    "filePath": "string",
    "fileName": "string",
    "taskCount": 0,
    "unZip": true
    }    
    ```
- 批量下载
    ```json
    // [post] /api/Manage/download/list
    [
        {
            "url": "string",
            "filePath": "string",
            "fileName": "string",
            "taskCount": 0,
            "unZip": true
        }
    ]    
    ```
