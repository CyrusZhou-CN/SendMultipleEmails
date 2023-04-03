import request from '@/utils/request'

// 注册用户
export function signUp(userId, password) {
  return request({
    url: '/user',
    method: 'post',
    data: {
      userId,
      password
    }
  })
}

// 登陆
export function signIn(userId, password) {
  return request({
    url: '/user/sign-in',
    method: 'post',
    data: { userId, password }
  })
}

// 获取用户信息
export function getCurrentUserInfo() {
  return request({
    url: '/user/info',
    method: 'get'
  })
}

export function logout() {
  return request({
    url: '/user/logout',
    method: 'put'
  })
}

// 更新用户的头像
export function updateUserAvatar(avatar, userId) {
  return request({
    url: '/user/avatar',
    method: 'put',
    data: {
      avatar,
      userId
    }
  })
}
