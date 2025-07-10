// See https://aka.ms/new-console-template for more information

using LoadBalancer;

ServerManager serverManager = new ();

for (int index = 0; index < 3; index++)
{
	Server server = new ($"Server {index + 1}");
	serverManager.AddServer(server);
}

List<Server> serverList = serverManager.GetAllServers().Where(server => server.Status).ToList();
ILoadBalancerStrategy strategy = new RoundRobinStrategy(serverList);
// serverList[0].Status = false;
for (int request = 0; request < 10; request++)
{
	strategy.ProcessRequests();
}