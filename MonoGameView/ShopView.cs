using System;
using System.Collections.Generic;
using System.Linq;
using GolemCore;
using GolemCore.Models.Part;

namespace MonoGameView;

public class ShopView
{
    private readonly Shop _shop;
    private readonly ClusterManager _clusterManager;
    private List<DraggablePartCluster> _shopClusters;

    public ShopView(Shop shop, ClusterManager clusterManager)
    {
        _shop = shop;
        _clusterManager = clusterManager;
        _shopClusters = new List<DraggablePartCluster>();
        _clusterManager.ClusterSocketed += (sender, eventArgs) =>
        {
            if (_shopClusters.Contains(eventArgs.Cluster))
            {
                BuyPart(eventArgs.Cluster);
            }
        };
        _shop.PlayerFundsChanged += DisableUnbuyableParts;
        shop.SetPartsForRound();
        GenerateShopParts();
    }

    private void GenerateShopParts()
    {
        if (_shopClusters.Any())
        {
            _clusterManager.RemoveAllClusters(_shopClusters);
        }

        var parts = _shop.GetCurrentSelection();
        _shopClusters = new List<DraggablePartCluster>();
        for (int i = 0; i < parts.Count; i++)
        {
            var newCluster = _clusterManager.CreateCluster(parts[i], 80 + 150 * i, 500);
            _shopClusters.Add(newCluster);
        }
    }

    private void DisableUnbuyableParts()
    {
        Console.WriteLine("Disabling unbuyable parts!");
        foreach (var cluster in _shopClusters)
        {
            cluster.DraggingEnabled = _shop.CanAfford(cluster.Part);
        }
    }

    private void BuyPart(DraggablePartCluster cluster)
    {
        _shop.BuyPartAtIndex(_shopClusters.IndexOf(cluster));
        _shopClusters.Remove(cluster); // Relinquish that cluster since it is no longer part of the shop
        GenerateShopParts();
    }

    public void SellCluster(DraggablePartCluster cluster, Part part)
    {
        if (_shopClusters.Contains(cluster))
        {
            cluster.RevertToPositionBeforeDrag();
            return; // Can't sell parts from the shop!
        }
        _shop.SellPart(part);
        _clusterManager.RemoveCluster(cluster);
    }

    public string GetPlayerFunds()
    {
        return _shop.PlayerFunds.ToString();
    }

    public void Reroll()
    {
        _shop.SetPartsForRound();
        GenerateShopParts();
    }
}