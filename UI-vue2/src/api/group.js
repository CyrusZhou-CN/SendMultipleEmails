import request from '@/utils/request'

export function getGroups(groupType) {
  return request({
    url: '/email-box-group/all',
    method: 'get',
    params: {
      groupType
    }
  })
}

export function newGroup(data) {
  return request({
    url: '/email-box-group',
    method: 'post',
    data
  })
}

// 通过组id删除组
export function deleteGroupById(groupId) {
  return request({
    url: `/email-box-group/${groupId}`,
    method: 'delete'
  })
}

export function modifyGroup(groupId, data) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/email-box-group/${groupId}`,
    method: 'put',
    data: {
      groupId,
      ...data
    }
  })
}

// 获取组下的邮箱
// emailType: inbox,outbox
export function getEmailsCount(groupId, filter) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/email-box-group/${groupId}/email-box/count`,
    method: 'post',
    data: {
      filter
    }
  })
}

// 获取组下的邮箱
export function getEmails(groupId, filter, pagination) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/email-box-group/${groupId}/email-box/list`,
    method: 'post',
    data: { filter, pagination }
  })
}

// 在组中新建邮箱
export function newEmail(data) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/email-box-group/${data.groupId}/email`,
    method: 'post',
    data
  })
}

// 批量新建邮箱
export function newEmails(groupId, data) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/email-box-group/${groupId}/email-box`,
    method: 'post',
    data
  })
}

// 删除邮箱
export function deleteEmail(id) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/email-box/${id}`,
    method: 'delete'
  })
}

// 删除组下面所有的邮箱
export function deleteEmails(groupId) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/email-box-group/${groupId}/email-box`,
    method: 'delete'
  })
}

// 修改邮箱
export function modifyEmail(emailId, data) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/email-box/${emailId}`,
    method: 'put',
    data
  })
}

// 修改发件箱设置
export function updateSendEmailSettings(emailId, data) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/email-box/${emailId}/settings`,
    method: 'put',
    data
  })
}
