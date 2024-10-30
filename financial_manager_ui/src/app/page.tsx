"use client";

import React, { useEffect, useState } from 'react';
import Link from 'next/link';
import { apiGetRequest } from '@/app/api/getLoans/route';
import { getTotalLoanAmount } from '@/app/api/getTotalLoanAmount/route';

const Home = () => {
    const [loans, setLoans] = useState([]);
    const [totalAmount, setTotalAmount] = useState(null); // Initialize to null to distinguish loading state

    useEffect(() => {
        // Use an IIFE to handle async logic
        (async () => {
            try {
                const loansData = await apiGetRequest('/GetLoans');
                setLoans(loansData || []);

                const total = await getTotalLoanAmount();
                setTotalAmount(total || 0);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        })();
    }, []);


    return (
        <div className="body">
            <div className="title">
                <h1>Welcome to Loan Tracker</h1>
            </div>
            <div className="totalOwed">
                <h3>Total Owed: ${totalAmount !== null ? totalAmount : 'Loading...'}</h3>
            </div>
            <div className="tableTitle">
                <h4>Loans</h4>
            </div>
            <div className="loanTableContainer">
                <table className="loanTable">
                    <thead>
                    <tr>
                        <th>Loan ID</th>
                        <th>User Id</th>
                        <th>Loan Name</th>
                        <th>Loan Amount</th>
                    </tr>
                    </thead>
                    <tbody>
                    {loans.length > 0 ? (
                        loans.map(({ LoanAmount, LoanId, LoanName, UserId }) => (
                            <tr key={LoanId}>
                                <td>{LoanId}</td>
                                <td>{UserId}</td>
                                <td>{LoanName}</td>
                                <td>${LoanAmount}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            
                        </tr>
                    )}
                    </tbody>
                </table>
            </div>
            <div className="buttonContainer">
                <Link href="/addLoan">
                    <button className="button">Add Loan</button>
                </Link>
            </div>
        </div>
    );
};

export default Home;