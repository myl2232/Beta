local basetype = require "BaseType"
local panel = basetype.class()

function panel:ctor(x)	-- 定义构造函数
	print("panel ctor")
	self.x=x
end
 
function panel:print_x()	-- 定义一个成员函数 :print_x
	print(self.x)
end
 
function panel:hello()	-- 定义另一个成员函数 :hello
	print("hello panel")
end