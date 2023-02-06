resource "random_pet" "prefix" {}

resource "azurerm_kubernetes_cluster" "kseCluster" {
  name                = var.clusterName
  location            = azurerm_resource_group.kse_rg.location
  resource_group_name = azurerm_resource_group.kse_rg.name
  tags = var.tags

  dns_prefix          = "kse-${random_pet.prefix.id}"
  kubernetes_version  = "1.24"

  default_node_pool {
    name            = "default"
    max_count = 3
    min_count = 1
    node_count      = 1
    vm_size         = "Standard_B2s"
    os_disk_size_gb = 30
    enable_auto_scaling = true
  }

  network_profile{
    network_plugin = "azure"
    load_balancer_sku = "standard"
  }
  
  identity {
    type = "SystemAssigned"
  }

  role_based_access_control_enabled = true
}
