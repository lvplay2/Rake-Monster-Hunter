using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace CompleteProject
{
	public class Purchaser : MonoBehaviour, IStoreListener
	{
		public static string thispack;

		public static Purchaser _instance;

		private static IStoreController m_StoreController;

		private static IExtensionProvider m_StoreExtensionProvider;

		public int[] itemid;

		private int thisid;

		public static string noAds = "noads";

		private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

		private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

		private int currproductId;

		public static Purchaser Instance
		{
			get
			{
				return _instance;
			}
		}

		private void Start()
		{
			_instance = this;
			if (m_StoreController == null)
			{
				InitializePurchasing();
			}
		}

		public void InitializePurchasing()
		{
			if (!IsInitialized())
			{
				ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
				configurationBuilder.AddProduct(noAds, ProductType.NonConsumable);
				UnityPurchasing.Initialize(this, configurationBuilder);
			}
		}

		private bool IsInitialized()
		{
			return m_StoreController != null && m_StoreExtensionProvider != null;
		}

		public void BuyConsumable(string cons)
		{
			BuyProductID(cons);
		}

		public void BuySubscription()
		{
		}

		public void BuyProductID(string productId)
		{
			if (IsInitialized())
			{
				Product product = m_StoreController.products.WithID(productId);
				if (product != null && product.availableToPurchase)
				{
					Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
					m_StoreController.InitiatePurchase(product);
				}
				else
				{
					Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			}
			else
			{
				Debug.Log("BuyProductID FAIL. Not initialized.");
			}
		}

		public void RestorePurchases()
		{
			if (!IsInitialized())
			{
				Debug.Log("RestorePurchases FAIL. Not initialized.");
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
			{
				Debug.Log("RestorePurchases started ...");
				IAppleExtensions extension = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
				extension.RestoreTransactions(delegate(bool result)
				{
					Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
				});
			}
			else
			{
				Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			Debug.Log("OnInitialized: PASS");
			m_StoreController = controller;
			m_StoreExtensionProvider = extensions;
			if (m_StoreController.products.WithID("noads").hasReceipt)
			{
				AdverController.noAds = true;
			}
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
		{
			if (string.Equals(args.purchasedProduct.definition.id, noAds, StringComparison.Ordinal))
			{
				Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
				AdverController.noAds = true;
			}
			else
			{
				Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
			}
			return PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
		}
	}
}
