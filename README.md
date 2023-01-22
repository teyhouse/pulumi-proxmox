# 💡  Pulumi Proxmox Container Example (C#)
Since practical examples for the Pulumi Proxmox-Provider are kind of limited right now, this is meant as a simple showcase. This projects was crated based on the offical Pulumi-C# Template. 
  
# 📃 Requirements
- .NET Runtime >= 6
https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu 
- Pulumi: 
https://www.pulumi.com/docs/get-started/install/
- Pulumi Proxmox Provider: 
```dotnet add package Pulumi.ProxmoxVE --version 2.2.0```  
(check for newer version on: https://github.com/muhlba91/pulumi-proxmoxve)
- Proxmox root-credentials

# 🚫 Limitations
As of right now the Pulumi-Provider has not implemented all the Terraform-Provider properties.
For example containers are missing the option for disk-size or nested-mode.
There might be more, so always check the APIs first.

# 📖 API
https://registry.terraform.io/providers/bpg/proxmox/latest/docs/resources/virtual_environment_container
https://www.pulumi.com/registry/packages/proxmoxve/api-docs/ct/container/

# 🛠 How to run 
Preview changes:  
```pulumi preview```
  
Run:  
```pulumi up```
  
Revert changes:  
```pulumi destroy```

![screenshot](pulumi.png?raw=true)