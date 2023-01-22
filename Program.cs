using System.Collections.Generic;
using Pulumi;
using Pulumi.ProxmoxVE;

return await Deployment.RunAsync(() =>
{
   /*
      For API-Documentation please check:
      https://registry.terraform.io/providers/bpg/proxmox/latest/docs/resources/virtual_environment_container
      https://www.pulumi.com/registry/packages/proxmoxve/api-docs/ct/container/
   */
   
   const int containercount = 3;

   //Best Practice: Read those values from ENV - Environment.GetEnvironmentVariable("YOURVALUE")
   Pulumi.ProxmoxVE.Inputs.ProviderVirtualEnvironmentArgs args = new Pulumi.ProxmoxVE.Inputs.ProviderVirtualEnvironmentArgs();
   args.Endpoint = "https://192.168.237.89:8006";
   args.Insecure = true;
   args.Username = "root@pam";
   args.Password = "proxmoxrootpw";

   Pulumi.ProxmoxVE.ProviderArgs pargs = new ProviderArgs();
   pargs.VirtualEnvironment = args;

   Pulumi.ProxmoxVE.Provider prox = new Pulumi.ProxmoxVE.Provider("proxmoxve", pargs);

   //Prepare Container Properties
   Pulumi.ProxmoxVE.CT.ContainerArgs ctargs = new Pulumi.ProxmoxVE.CT.ContainerArgs();
   Pulumi.ProxmoxVE.CT.Inputs.ContainerInitializationArgs ctinit = new Pulumi.ProxmoxVE.CT.Inputs.ContainerInitializationArgs();

   Pulumi.ProxmoxVE.CT.Inputs.ContainerInitializationIpConfigArgs ipargs = new Pulumi.ProxmoxVE.CT.Inputs.ContainerInitializationIpConfigArgs();
   Pulumi.ProxmoxVE.CT.Inputs.ContainerInitializationIpConfigIpv4Args ipfargs = new Pulumi.ProxmoxVE.CT.Inputs.ContainerInitializationIpConfigIpv4Args();
   ipfargs.Address = "dhcp";
   ipargs.Ipv4 = ipfargs;   

   Pulumi.ProxmoxVE.CT.Inputs.ContainerInitializationUserAccountArgs useracc = new Pulumi.ProxmoxVE.CT.Inputs.ContainerInitializationUserAccountArgs();
   useracc.Password = "secretcontainerrootpassword";
   useracc.Keys.Add("ssh-rsa YOUR SSH-PUB-KEY");
   ctinit.UserAccount = useracc;
   ctinit.IpConfigs = ipargs;      

   //Get template path from proxmox shell: pveam list local
   Pulumi.ProxmoxVE.CT.Inputs.ContainerOperatingSystemArgs osargs = new Pulumi.ProxmoxVE.CT.Inputs.ContainerOperatingSystemArgs();
   osargs.TemplateFileId = "local:vztmpl/ubuntu-22.04-standard_22.04-1_amd64.tar.zst";
   osargs.Type = "ubuntu";
   
   //Note: The current Pulumi-Proxmox Provider (Version 2.2.0) is missing the option for disk size 
   Pulumi.ProxmoxVE.CT.Inputs.ContainerDiskArgs diskargs = new Pulumi.ProxmoxVE.CT.Inputs.ContainerDiskArgs();
   diskargs.DatastoreId = "local-lvm";
   
   Pulumi.ProxmoxVE.CT.Inputs.ContainerCpuArgs cpuargs = new Pulumi.ProxmoxVE.CT.Inputs.ContainerCpuArgs();
   cpuargs.Cores = 1;
   cpuargs.Units = 1;

   Pulumi.ProxmoxVE.CT.Inputs.ContainerMemoryArgs memargs = new Pulumi.ProxmoxVE.CT.Inputs.ContainerMemoryArgs();
   memargs.Dedicated = 512;
   memargs.Swap = 512;

   Pulumi.ProxmoxVE.CT.Inputs.ContainerNetworkInterfaceArgs netargs = new Pulumi.ProxmoxVE.CT.Inputs.ContainerNetworkInterfaceArgs();
   netargs.Name = "eth0";
   netargs.Enabled = true;
   netargs.Bridge = "vmbr0";
   
   //Combine all Container Properties

   //Its better to not provide any Container IDs - Proxmox will auto-generate them for you
   //ctargs.VmId = 103;

   ctargs.NodeName = "pve";
   ctargs.OperatingSystem = osargs;
   ctargs.Disk = diskargs;
   ctargs.Cpu = cpuargs;
   ctargs.NetworkInterfaces = netargs;
   ctargs.Initialization = ctinit;
   ctargs.Started = true;
   
   //Build the actually Linux Container(s)
   for(int i = 1; i <= containercount; i++){
      Pulumi.ProxmoxVE.CT.Container ct = new Pulumi.ProxmoxVE.CT.Container("Pulumi"+i, ctargs, new CustomResourceOptions{Provider = prox});
   }  
   
   // Export outputs here
   return new Dictionary<string, object?>
   {
      ["Status"] = "done"
   };
});
