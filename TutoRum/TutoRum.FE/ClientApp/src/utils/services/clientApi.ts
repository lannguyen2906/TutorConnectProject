// clientApi.ts (API tự tạo)

import { ContentType, HttpClient, RequestParams } from "./Api";

export class ClientApi<SecurityDataType extends unknown> {
  http: HttpClient<SecurityDataType>;

  constructor(http: HttpClient<SecurityDataType>) {
    this.http = http;
  }

  auth = (
    body: { sessionToken: string; expiresAt: string },
    params: RequestParams = {}
  ) =>
    this.http.request<void, any>({
      path: `/api/auth`,
      method: "POST",
      body,
      secure: true,
      type: ContentType.Json,
      baseURL: "/",
      ...params,
    });

  logoutFromNextClientToNextServer = (
    force?: boolean | undefined,
    signal?: AbortSignal | undefined,
    params: RequestParams = {}
  ) =>
    this.http.request<void, any>({
      path: `/api/auth/logout`,
      method: "POST",
      body: {
        force,
      },
      secure: true,
      type: ContentType.Json,
      baseURL: "/",
      signal,
      ...params,
    });
}
