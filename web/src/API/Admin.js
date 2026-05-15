import serviceAxios from '../utils/serviceAxios'

//获取测试用例列表
export const GetTestCaseList = (params, data) =>
  serviceAxios({
    url: '/Common/GetTestCaseList',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

//创建测试用例
export const AddNewTestCase = (params, data) =>
  serviceAxios({
    url: '/Common/AddNewTestCase',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

//更新测试用例
export const UpdateTestCase = (params, data) =>
  serviceAxios({
    url: '/Common/UpdateTestCase',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

  //更新测试用例
export const UpdatePreconditions = (params, data) =>
  serviceAxios({
    url: '/Common/UpdatePreconditions',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

//删除测试用例
export const DeleteTestCase = (params, data) =>
  serviceAxios({
    url: '/Common/DeleteTestCase',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

  //获取测试步骤列表
export const GetTestStepDatas = (params, data) =>
  serviceAxios({
    url: '/TestStep/GetTestStepDatas',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

  //获取记录信号
  export const GetTestRecodeDatas = (params, data) =>
  serviceAxios({
    url: '/Common/GetTestRecodeDatas',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })
  
  //更新记录信号
  export const UpdateTestRecodeDatas = (params, data) =>
  serviceAxios({
    url: '/Common/UpdateTestRecodeDatas',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })
  
    //查询用例对应的监控信号
  export const GetOneTestRecodeDatas = (params, data) =>
  serviceAxios({
    url: '/Common/GetOneTestRecodeDatas',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

      //更新列车信息
  export const UpdateTrainDatas = (params, data) =>
  serviceAxios({
    url: '/Common/UpdateTrainDatas',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

//创建测试步骤
export const CreateTestStep = (params, data) =>
  serviceAxios({
    url: '/TestStep/CreateTestStep',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

//更新测试步骤
export const UpdateTestStep = (params, data) =>
  serviceAxios({
    url: '/TestStep/UpdateTestStep',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

//删除测试步骤
export const DeleteTestStep = (params, data) =>
  serviceAxios({
    url: '/TestStep/DeleteTestStep',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })

  
//执行测试用例
export const ExecuteTestCase = (params, data) =>
  serviceAxios({
    url: '/Common/ExecuteTestCase',
    headers: {
      'Content-Type': 'application/json',
    },
    method: 'post',
    params,
    data,
  })
