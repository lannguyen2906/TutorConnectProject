import React, { createContext, useContext, useState, ReactNode } from "react";

// Định nghĩa type cho role của người dùng
type UserRole = "tutor" | "learner";

// Định nghĩa interface cho context value
interface ClassroomContextType {
  classroomId: string | null;
  userRole: UserRole | null;
  updateClassroom: (id: string, role: UserRole) => void;
}

// Khởi tạo context với giá trị mặc định là undefined
const ClassroomContext = createContext<ClassroomContextType | undefined>(
  undefined
);

// Custom provider cho context
interface ClassroomProviderProps {
  children: ReactNode;
}

export const ClassroomProvider: React.FC<ClassroomProviderProps> = ({
  children,
}) => {
  const [classroomId, setClassroomId] = useState<string | null>(null);
  const [userRole, setUserRole] = useState<UserRole | null>(null);

  const updateClassroom = (id: string, role: UserRole) => {
    setClassroomId(id);
    setUserRole(role);
  };

  return (
    <ClassroomContext.Provider
      value={{ classroomId, userRole, updateClassroom }}
    >
      {children}
    </ClassroomContext.Provider>
  );
};

export const useClassroomContext = (): ClassroomContextType => {
  const context = useContext(ClassroomContext);
  if (!context) {
    throw new Error(
      "useClassroomContext phải được sử dụng trong ClassroomProvider"
    );
  }
  return context;
};
