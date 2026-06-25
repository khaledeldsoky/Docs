# if you apply a deployment and want to check the env variable dry to run it with docker
```sh
docker run -e PORT=8080 \
-e PRODUCT_CATALOG_SERVICE_ADDR=productcatalogservice:3550 \
-e CURRENCY_SERVICE_ADDR=currencyservice:7000 \
-e CART_SERVICE_ADDR=cartservice:7070 \
-e RECOMMENDATION_SERVICE_ADDR=recommendationservice:8080 \
-e SHIPPING_SERVICE_ADDR=shippingservice:50052 \
-e CHECKOUT_SERVICE_ADDR=checkoutservice:5050 \
-e AD_SERVICE_ADDR=adservice:9555 \
us-central1-docker.pkg.dev/google-samples/microservices-demo/frontend:v0.10.1
```

```sh
docker run -e PORT=7000 -p 7000:7000 us-central1-docker.pkg.dev/google-samples/microservices-demo/currencyservice:v0.10.1
```
---------------------------- ----------------------------
# solution fot loadbalancer

```sh
kubectl apply -f https://raw.githubusercontent.com/metallb/metallb/v0.13.10/manifests/metallb.yaml
```
---------------------------- ----------------------------

# there is problem **FailedCreatePodSandBox**
and i use ```sh kubectl apply -f https://docs.projectcalico.org/manifests/calico.yaml```

search for calico or CNI
---------------------------- ----------------------------

