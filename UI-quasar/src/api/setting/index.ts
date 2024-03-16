import request from "@/utils/request";
import { AxiosPromise } from "axios";
import { SystemLoginSettings } from "./types";

/**
 * 获取版本和icp备案
 */
export function getSystemLoginSettings(): AxiosPromise<SystemLoginSettings> {
  return request({
    url: "/api/v1/system-setting/login-settings",
    method: "get",
  });
}
