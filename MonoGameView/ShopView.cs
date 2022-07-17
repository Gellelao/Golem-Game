using System;
using System.Collections.Generic;
using System.Linq;
using GolemCore;
using GolemCore.Models.Part;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameView.Events;

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
            _shopClusters.Add(_clusterManager.CreateCluster(parts[i], 80 + 150*i, 500));
        }
    }

    private void BuyPart()
    {
        // Can take a part or cluster
        // remove it from local list and clustermanager, inform shop (to remove from shop selection and deduct funds)
    }

    public void SellCluster(DraggablePartCluster cluster, Part part)
    {
        _shop.SellPart(part);
        _clusterManager.RemoveCluster(cluster);
    }
}