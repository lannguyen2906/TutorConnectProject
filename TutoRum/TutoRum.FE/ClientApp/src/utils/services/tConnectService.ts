import {
  attachCommonErrorInterceptor,
  attachStatusCodeErrorInterceptor,
} from "./interceptors";
import { Api, HttpClient } from "./Api";
import { ClientApi } from "./clientApi";

const httpClient = new HttpClient({
  withCredentials: true,
  timeout: 30 * 1000,
});

// Order is important here. These interceptors will be executed in reverse order of attachment
attachCommonErrorInterceptor(httpClient.instance);
attachStatusCodeErrorInterceptor(httpClient.instance);

export const tConnectService = new Api(httpClient);
export const clientService = new ClientApi(httpClient);
