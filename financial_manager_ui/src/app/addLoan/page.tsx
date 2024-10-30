"use client";

import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import { apiPostRequest } from '@/app/api/addLoan/route';

const AddLoan = () => {
    const [loanName, setLoanName] = useState('');
    const [loanAmount, setLoanAmount] = useState('');
    const [userId, setUserId] = useState(''); // New state for User ID
    const [loanId, setLoanId] = useState(''); // New state for Loan ID

    const router = useRouter();

    const handleSubmit = async (e:any) => {
        e.preventDefault();

        const newLoan = {
            LoanId: parseInt(loanId), // Parse to integer if necessary
            UserId: parseInt(userId),  // Parse to integer if necessary
            LoanName: loanName,
            LoanAmount: parseFloat(loanAmount),
        };

        const result = await apiPostRequest('/AddLoan', newLoan);

        if (result) {
            router.push('/');
        }
    };

    return (
        <div className="bodyLoan">
            <div className="container">
                <h2>Add Loan</h2>
                <form onSubmit={handleSubmit}>
                    <label className="label">User ID:</label>
                    <input
                        className="input"
                        type="number"
                        placeholder="User ID"
                        value={userId}
                        onChange={(e) => setUserId(e.target.value)}
                    />

                    <label className="label">Loan ID:</label>
                    <input
                        className="input"
                        type="number"
                        placeholder="Loan ID"
                        value={loanId}
                        onChange={(e) => setLoanId(e.target.value)}
                    />

                    <label className="label">Loan Name:</label>
                    <input
                        className="input"
                        type="text"
                        placeholder="Loan Name"
                        value={loanName}
                        onChange={(e) => setLoanName(e.target.value)}
                    />

                    <label className="label">Loan Amount:</label>
                    <input
                        className="input"
                        type="number"
                        placeholder="Loan Amount"
                        value={loanAmount}
                        onChange={(e) => setLoanAmount(e.target.value)}
                    />

                    <button type="submit" className="buttonAddLoan">ADD</button>
                </form>
            </div>
        </div>
    );
};

export default AddLoan;