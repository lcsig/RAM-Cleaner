# RAM Cleaner ...

#### Simple program that cleans the RAM of each process
##### Designed to work at windows startup to clean the RAM repeatedly after certain time interval



## Programming ...

```
BOOL SetProcessWorkingSetSize(
  HANDLE hProcess,
  SIZE_T dwMinimumWorkingSetSize,
  SIZE_T dwMaximumWorkingSetSize
);
```

Calling SetProcessWorkingSetSize API with ( hProcess, -1, -1 ) will removes as many pages as possible from the working set of the specified process. Which is the core idea of this project. 
