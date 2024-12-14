import { cacheKeysUtil } from "@/utils/cache/cacheKeysUtil";
import { AddressResponse } from "@/utils/other/mapper";
import { FullProfile } from "@/utils/schemaValidations/account.schema";
import { UserProfileBodyType } from "@/utils/schemaValidations/user.profile.schema";
import { SignInModel, SignInResponseDto } from "@/utils/services/Api";
import { tConnectService } from "@/utils/services/tConnectService";
import axios from "axios";
import { useQuery, UseQueryResult } from "react-query";

export const useLogin = (loginModel: SignInModel): UseQueryResult =>
  useQuery(cacheKeysUtil.login(loginModel), async (): Promise<any> => {
    const response = await tConnectService.api.accountsSignInCreate(loginModel);
    const { data } = response;
    return data;
  });

export const useProfileCurrent = (): UseQueryResult<FullProfile> =>
  useQuery(
    cacheKeysUtil.currentProfile(),
    async (): Promise<FullProfile | undefined> => {
      const response = await tConnectService.api.accountsCurrentList();
      const user = response.data.data;

      if (!user) {
        await fetch("/api/auth/logout", {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
          },
        });
        return;
      }

      // Sử dụng thông tin từ SignInResponseDto để tạo profile mặc định
      const defaultProfile = {
        id: user.id || "",
        fullName: user.fullname || "",
        dateOfBirth: user.dob || "",
        address: user.addressDetail || "",
        city: user.cityId || "",
        district: user.districtId || "",
        ward: user.wardId || "",
        email: user.email || "",
        gender: user.gender ? "male" : "female",
        phoneNumber: user.phoneNumber || "",
        avatarUrl: user.avatarUrl || "",
        roles: user.roles || [],
        balance: user.balance || 0,
      };

      return { ...defaultProfile };
    }
  );
