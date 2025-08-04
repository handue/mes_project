-- Oracle DB 데이터 확인 스크립트
-- Oracle DB Data Check Script

-- 실행 방법: docker exec -i oracle-mes sqlplus mes/mes123@//localhost:1521/XE
-- How to run: docker exec -i oracle-mes sqlplus mes/mes123@//localhost:1521/XE 

-- 1. 테이블 목록 확인
-- 1. Check table list
SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME;

-- 2. WORKORDERS 테이블 데이터 확인
-- 2. Check WORKORDERS table data count
SELECT COUNT(*) as WORKORDERS_COUNT FROM WORKORDERS;

-- 3. MACHINES 테이블 데이터 확인  
-- 3. Check MACHINES table data count
SELECT COUNT(*) as MACHINES_COUNT FROM MACHINES;

-- 4. INVENTORY 테이블 데이터 확인
-- 4. Check INVENTORY table data count
SELECT COUNT(*) as INVENTORY_COUNT FROM INVENTORY;

-- 5. QUALITYCONTROL 테이블 데이터 확인
-- 5. Check QUALITYCONTROL table data count
SELECT COUNT(*) as QUALITYCONTROL_COUNT FROM QUALITYCONTROL;

-- 6. 샘플 데이터 확인 (WORKORDERS)
-- 6. Sample data from WORKORDERS
SELECT ORDERID, PRODUCTID, STATUS, QUANTITY FROM WORKORDERS WHERE ROWNUM <= 5;

-- 7. 샘플 데이터 확인 (MACHINES)
-- 7. Sample data from MACHINES
SELECT MACHINEID, NAME, STATUS, TYPE FROM MACHINES WHERE ROWNUM <= 5;

-- 8. 샘플 데이터 확인 (INVENTORY)
-- 8. Sample data from INVENTORY
SELECT ITEMID, NAME, QUANTITY, COST FROM INVENTORY WHERE ROWNUM <= 5;

-- 9. 테이블 구조 확인
-- 9. Check table structure
DESC WORKORDERS;
DESC MACHINES;
DESC INVENTORY;
DESC QUALITYCONTROL;

-- 10. 사용자 정보 확인
-- 10. Check user info
SELECT USER, SYSDATE FROM DUAL; 