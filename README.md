# RAM Cleaner ...
A program to optimize your system's performance, and clears the RAM of every process. Configured to initiate with Windows startup, it ensures continuous RAM cleaning at specified intervals you determine. 


## Technical Details 
This project makes use of the SetProcessWorkingSetSize Windows API, which when invoked with (hProcess, -1, -1), minimizes the target process's working set size by deleting the greatest number of memory pages. The idea at the heart of this project is this API call.
```
BOOL SetProcessWorkingSetSize(
  HANDLE hProcess,
  SIZE_T dwMinimumWorkingSetSize,
  SIZE_T dwMaximumWorkingSetSize
);
```

## How to Install
It is pretty simple, from the release page, download the exe attached file and run it. 

## Buy me a Coffee: 
BTC: bc1q2kqvggm552h0csyr0awa2zepdapxdqnacw0z5w

![BTC](https://raw.githubusercontent.com/lcsig/API-Hooking/refs/heads/master/img/btc.png)
